using System.Text;

namespace Differ.CustomBinders.Deserializer;


internal class Base64JsonToStringDeserializer(ILogger<Base64JsonToObjectDeserializer> logger) 
    : Base64JsonDeserializerBase(logger), IBase64JsonDeserializer
{
    public async Task<Result<Object?>> DeserializeAsync(Stream stream,
        Type targetType, Encoding encoding)
    {
        ArgumentNullException.ThrowIfNull(stream);
        
        var base64Result = await TryGetBase64StringAsync(stream);
        if (!base64Result.IsSuccess) return new Result<object?>(null, base64Result.ErrorMessage);

        var jsonBytesResult = TryGetJsonBytes(base64Result.Value!);
        if (!jsonBytesResult.IsSuccess) return new Result<object?>(null, jsonBytesResult.ErrorMessage);

        if (jsonBytesResult.Value is not { Length: 0 }) return new Result<object?>(encoding.GetString(jsonBytesResult.Value!), null);
        
        var message = "Base64 string is empty.";
        logger.LogInformation(message);
        return new Result<object?>(null,message);

    }
}