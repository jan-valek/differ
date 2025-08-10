using System.Text.Json;

namespace Differ.CustomBinders.Deserializer;

internal abstract class Base64JsonDeserializerBase(ILogger logger)
{
    protected async Task<Result<object?>> TryDeserializeObjectAsync(MemoryStream jsonStream, Type targetType)
    {
        object? boundModel;
        try
        {
            boundModel = await JsonSerializer.DeserializeAsync(jsonStream, targetType);
        }
        catch (Exception ex)
        {
            var message = "Error while deserializing.";
            logger.LogError(ex, message);
            return new Result<object?>(null, message);
        }
        return new Result<object?>(boundModel, null);
    }
    
    protected Result<byte[]> TryGetJsonBytes(string base64String)
    {
        byte[] jsonBytes;
        try
        {
            jsonBytes = Convert.FromBase64String(base64String);
        }
        catch (FormatException fe)
        {
            var message = "Input is not valid base64 string.";
            logger.LogError(fe, message);
            return new Result<byte[]>(null, message);
        }
        catch (ArgumentNullException ae)
        {
            var message = "Input is empty.";
            logger.LogError(ae, message);
            return new Result<byte[]>(null, message);
        }
        return new Result<byte[]>(jsonBytes, null);
    }
    
    protected async Task<Result<string>> TryGetBase64StringAsync(Stream stream)
    {
        string? base64String;
        try
        {
            base64String = await new StreamReader(stream).ReadToEndAsync();
        }
        catch (Exception e)
        {
            var message = "Error while reading stream";
            logger.LogError(e, message);
            return new Result<string> (null,message);
        }

        return new Result<string>(base64String, null);
    }
}