using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Differ.CustomBinders;

namespace Differ.IntegrationTests.Helpers;

public static class TestHelpers
{
    public static HttpContent CreateCustomContent(object input)
    {
        var json = JsonSerializer.Serialize(input);
        var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
        var content = new StringContent(base64, Encoding.UTF8);
        content.Headers.ContentType = new MediaTypeHeaderValue(CustomContentTypeModelBinderProvider.SupportedMediaType);
        return content;
    }
}