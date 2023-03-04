using MinimalAPI.Api.Dtos;
using Newtonsoft.Json;
using System.Net;
using Xunit;

namespace MinimalAPI.Tests;

public class IntegrationTests
{
    private readonly ApiApplication _app;
    private readonly HttpClient _client;

    public IntegrationTests()
    {
        // Arrange
        _app = new ApiApplication();
        _client = _app.CreateClient();
    }

    [Fact]
    public async Task Customers_ReturnsListOfCustomerDto()
    {
        // Act
        var response = await _client.GetAsync("/customers");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var jsonResponse = await response.Content.ReadAsStringAsync();
        Assert.NotEqual(jsonResponse, null);

        var customers = JsonConvert.DeserializeObject<List<CustomerDto>>(jsonResponse);
        Assert.NotEmpty(customers);
    }

    [Fact]
    public async Task Customer_ReturnsCustomerDtoWithCorrectCustomerId()
    {
        // Act
        Random rnd = new Random();
        int id = rnd.Next(1, 10);
        var response = await _client.GetAsync($"/customer?id={id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var jsonResponse = await response.Content.ReadAsStringAsync();
        Assert.NotEqual(jsonResponse, null);

        var customer = JsonConvert.DeserializeObject<CustomerDto>(jsonResponse);
        Assert.NotNull(customer);
    }

    [Fact]
    public async Task CustomersByCity_ReturnsListOfCustomerDtoWithCorrectCity()
    {
        // Act
        string city = "Paris";
        var response = await _client.GetAsync($"/customersbycity?city={city}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var jsonResponse = await response.Content.ReadAsStringAsync();
        Assert.NotEqual(jsonResponse, null);

        var customers = JsonConvert.DeserializeObject<List<CustomerDto>>(jsonResponse);
        Assert.True(customers?[0].City.Trim().ToLower() == city.Trim().ToLower());
    }
}
