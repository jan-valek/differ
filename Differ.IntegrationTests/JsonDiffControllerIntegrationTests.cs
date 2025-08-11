using System.Net;
using System.Text.Json;
using Differ.IntegrationTests.Fixtures;
using Differ.IntegrationTests.Helpers;
using Differ.Models;

namespace Differ.IntegrationTests;

public class JsonDiffControllerIntegrationTests : IClassFixture<TestFixture>
{
    private readonly HttpClient _client;

    public JsonDiffControllerIntegrationTests(TestFixture fixture)
    {
        _client = fixture.Client;
    }

    [Fact]
    public async Task GetDiff_WithEqualInputs_ReturnsEqualStatus()
    {
        // Arrange
        var id = Guid.NewGuid().ToString();
        var input = new { input = "testValue" };
        var content = TestHelpers.CreateCustomContent(input);
        
        await _client.PostAsync($"/v1/diff/{id}/left", content);
        await _client.PostAsync($"/v1/diff/{id}/right", content);

        // Act
        var response = await _client.GetAsync($"/v1/diff/{id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<DiffResult>(responseContent, new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        });
        
        Assert.NotNull(result);
        Assert.Equal("Inputs were equal.", result.Status);
    }

    [Fact]
    public async Task GetDiff_WithDifferentSizeInputs_ReturnsDifferentSizeStatus()
    {
        // Arrange
        var id = Guid.NewGuid().ToString();
        var leftInput = new { input = "shortValue" };
        var rightInput = new { input = "muchLongerValueThanLeft" };
        
        var leftContent = TestHelpers.CreateCustomContent(leftInput);
        var rightContent = TestHelpers.CreateCustomContent(rightInput);
        
        await _client.PostAsync($"/v1/diff/{id}/left", leftContent);
        await _client.PostAsync($"/v1/diff/{id}/right", rightContent);

        // Act
        var response = await _client.GetAsync($"/v1/diff/{id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<DiffResult>(responseContent, new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        });
        
        Assert.NotNull(result);
        Assert.Equal("Inputs are of different size.", result.Status);
    }

    [Fact]
    public async Task GetDiff_WithSameSizeDifferentInputs_ReturnsDifferences()
    {
        // Arrange
        var id = Guid.NewGuid().ToString();
        var leftInput = new { input = "testValue1" };
        var rightInput = new { input = "testValue2" };
        
        var leftContent = TestHelpers.CreateCustomContent(leftInput);
        var rightContent = TestHelpers.CreateCustomContent(rightInput);

        // Post left and right
        await _client.PostAsync($"/v1/diff/{id}/left", leftContent);
        await _client.PostAsync($"/v1/diff/{id}/right", rightContent);

        // Act
        var response = await _client.GetAsync($"/v1/diff/{id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<DiffResult>(responseContent, new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        });
        
        Assert.NotNull(result);
        Assert.Equal("Inputs have same size but are different.", result.Status);
        Assert.NotNull(result.Differences);
        Assert.NotEmpty(result.Differences);
        
        // Verify that differences contain offset and length
        foreach (var diff in result.Differences)
        {
            Assert.True(diff.Offset >= 0);
            Assert.True(diff.Length > 0);
        }
    }

    [Fact]
    public async Task GetDiff_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var id = Guid.NewGuid().ToString();

        // Act
        var response = await _client.GetAsync($"/v1/diff/{id}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PostLeft_WithInvalidBase64_ReturnsBadRequest()
    {
        // Arrange
        var id = Guid.NewGuid().ToString();
        var invalidBase64 = "This is not valid base64!@#$%";
        var content = new StringContent(invalidBase64, System.Text.Encoding.UTF8);
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/custom");

        // Act
        var response = await _client.PostAsync($"/v1/diff/{id}/left", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("not valid base64", responseContent, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task PostLeft_WithDataExceedingMaxLength_ReturnsBadRequest()
    {
        // Arrange
        var id = Guid.NewGuid().ToString();
        
        var longString = new string('x', 2001);
        var input = new { input = longString };
        var json = JsonSerializer.Serialize(input);
        var base64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(json));
        var content = new StringContent(base64, System.Text.Encoding.UTF8);
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/custom");

        // Act
        var response = await _client.PostAsync($"/v1/diff/{id}/left", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("maximum length", responseContent, StringComparison.OrdinalIgnoreCase);
    }
}