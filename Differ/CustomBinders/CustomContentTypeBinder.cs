using System.Text;
using Differ.CustomBinders.Deserializer;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Differ.CustomBinders;

/// <summary>
/// Custom modelbinder for application/custom media type.
/// </summary>
internal class CustomContentTypeBinder(IBase64JsonDeserializer deserializer) : IModelBinder
{
    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var request = bindingContext.HttpContext.Request;
        var contentType = request.GetTypedHeaders().ContentType;
        var encoding = contentType?.Encoding ?? Encoding.UTF8;

        var targetType = bindingContext.ModelType;

        var deserializationResult = await deserializer.DeserializeAsync(request.Body, targetType, encoding);
        if (!deserializationResult.IsSuccess)
        {
            bindingContext.ModelState.AddModelError(bindingContext.OriginalModelName, deserializationResult.ErrorMessage!);
            bindingContext.Result = ModelBindingResult.Failed();
            return;
        }

        bindingContext.Result = ModelBindingResult.Success(deserializationResult.Value);
    }
}