using System.Net;
using System.Net.Http.Json;
using MoviesAPI.Dtos;

namespace MoviesAPI.Tests;

[TestClass]
public sealed class ApiIntegrationTests
{
    private MoviesApiFactory _factory = null!;
    private HttpClient _client = null!;

    [TestInitialize]
    public void Initialize()
    {
        _factory = new MoviesApiFactory();
        _client = _factory.CreateClient(new()
        {
            BaseAddress = new Uri("https://localhost")
        });
    }

    [TestCleanup]
    public void Cleanup()
    {
        _client?.Dispose();
        _factory?.Dispose();
    }

    [TestMethod]
    public async Task GetGenres_ReturnsSeededGenres()
    {
        var response = await _client.GetAsync("/api/genres");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var genres = await response.Content.ReadFromJsonAsync<List<GenreDetailsDto>>();
        Assert.IsNotNull(genres);
        Assert.IsTrue(genres.Any(genre => genre.Name == "Action"));
        Assert.IsTrue(genres.Any(genre => genre.Name == "Drama"));
    }

    [TestMethod]
    public async Task GetMovies_ReturnsPagedMoviesOrderedByRateDescending()
    {
        var response = await _client.GetAsync("/api/movies");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<PagedResult<MovieDetailsDto>>();
        Assert.IsNotNull(result);
        Assert.AreEqual(3, result.TotalCount);
        Assert.AreEqual(1, result.Page);
        Assert.AreEqual(10, result.PageSize);
        Assert.AreEqual(1, result.TotalPages);
        Assert.AreEqual("The Godfather", result.Items[0].Title);
        Assert.IsTrue(result.Items.Any(movie => movie.Title == "The Matrix" && movie.GenreName == "Action"));
    }

    [TestMethod]
    public async Task GetMovies_WithSearch_ReturnsMatchingMoviesOnly()
    {
        var result = await _client.GetFromJsonAsync<PagedResult<MovieDetailsDto>>("/api/movies?search=Matrix");

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.TotalCount);
        Assert.AreEqual(1, result.Items.Count);
        Assert.AreEqual("The Matrix", result.Items[0].Title);
    }

    [TestMethod]
    public async Task GetMovies_WithPagingAndSorting_ReturnsExpectedPage()
    {
        var result = await _client.GetFromJsonAsync<PagedResult<MovieDetailsDto>>(
            "/api/movies?page=2&pageSize=1&sortBy=Title&sortDirection=Asc");

        Assert.IsNotNull(result);
        Assert.AreEqual(3, result.TotalCount);
        Assert.AreEqual(3, result.TotalPages);
        Assert.AreEqual(2, result.Page);
        Assert.AreEqual(1, result.Items.Count);
        Assert.AreEqual("The Matrix", result.Items[0].Title);
    }

    [TestMethod]
    public async Task GetMovies_WithInvalidPageSize_ReturnsBadRequest()
    {
        var response = await _client.GetAsync("/api/movies?pageSize=1000");

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [TestMethod]
    public async Task CreateGenre_ReturnsCreatedAndPersistsResource()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/genres", new GenreDto
        {
            Name = "Sci-Fi"
        });

        Assert.AreEqual(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.IsNotNull(createResponse.Headers.Location);

        var created = await createResponse.Content.ReadFromJsonAsync<GenreDetailsDto>();
        Assert.IsNotNull(created);
        Assert.AreEqual("Sci-Fi", created.Name);

        var getResponse = await _client.GetAsync($"/api/genres/{created.Id}");
        Assert.AreEqual(HttpStatusCode.OK, getResponse.StatusCode);
    }

    [TestMethod]
    public async Task CreateMovie_WithInvalidGenre_ReturnsBadRequest()
    {
        using var form = BuildMovieForm(genreId: byte.MaxValue);

        var response = await _client.PostAsync("/api/movies", form);

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [TestMethod]
    public async Task CreateMovie_WithValidData_ReturnsCreated()
    {
        var genres = await _client.GetFromJsonAsync<List<GenreDetailsDto>>("/api/genres");
        Assert.IsNotNull(genres);

        var actionGenre = genres.Single(genre => genre.Name == "Action");
        using var form = BuildMovieForm(actionGenre.Id);

        var response = await _client.PostAsync("/api/movies", form);

        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        Assert.IsNotNull(response.Headers.Location);

        var created = await response.Content.ReadFromJsonAsync<MovieDetailsDto>();
        Assert.IsNotNull(created);
        Assert.AreEqual("Integration Test Movie", created.Title);
        Assert.AreEqual("Action", created.GenreName);
    }

    [TestMethod]
    public async Task DeleteUnknownMovie_ReturnsNotFound()
    {
        var response = await _client.DeleteAsync("/api/movies/999999");

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    private static MultipartFormDataContent BuildMovieForm(byte genreId)
    {
        var form = new MultipartFormDataContent
        {
            { new StringContent("Integration Test Movie"), "Title" },
            { new StringContent("2025"), "Year" },
            { new StringContent("8.5"), "Rate" },
            { new StringContent("Created by the HTTP integration test suite."), "Storeline" },
            { new StringContent(genreId.ToString()), "GenreId" }
        };

        var poster = new ByteArrayContent(new byte[] { 0xFF, 0xD8, 0xFF, 0xD9 });
        poster.Headers.ContentType = new("image/jpeg");
        form.Add(poster, "Poster", "poster.jpg");

        return form;
    }
}
