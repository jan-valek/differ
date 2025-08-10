using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Differ.CustomBinders;

public class CustomContentTypeProvider:IModelBinderProvider
{
    public const string SupportedMediaType = "application/custom";
    
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        // because dotnet, doesnt allow add the binder provider by type from DI.
        var logger = context.Services.GetRequiredService<ILogger<CustomContentTypeProvider>>();
        
        if (context.BindingInfo.BindingSource != BindingSource.Body)
        {
            logger.LogDebug("Binding source {source} ignored.",context.BindingInfo.BindingSource);
            return null;
        }
        
        var httpContext = context.Services.GetService<IHttpContextAccessor>()?.HttpContext;

        if (httpContext is null)
        {
            logger.LogDebug("HttpContext is null");
            return null;
        }
        
        var contentType = httpContext.Request.ContentType;

        if (string.IsNullOrEmpty(contentType) || string.CompareOrdinal(SupportedMediaType, contentType?.ToLower()) != 0)
        {
            return null;
        }
        
        return context.Services.GetRequiredService<CustomContentTypeBinder>();
    }
}