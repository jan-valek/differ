using System.Text;
using Differ.Models;

namespace Differ.CustomBinders.Deserializer;

/// <summary>
/// Deserialize base64encoded json to targettype requested by model binder. 
/// </summary>
internal class Base64JsonToObjectDeserializer(ILogger<Base64JsonToObjectDeserializer> logger) : Base64JsonDeserializerBase(logger), IBase64JsonDeserializer
{
    public async Task<Result<Object?>> DeserializeAsync(Stream stream, Type targetType, Encoding encoding)
    {
        ArgumentNullException.ThrowIfNull(stream);
        
        var base64Result = await TryGetBase64StringAsync(stream);
        if (!base64Result.IsSuccess) return new Result<object?>(null,base64Result.ErrorMessage);

        var jsonBytesResult = TryGetJsonBytes(base64Result.Value!);
        if (!jsonBytesResult.IsSuccess) return new Result<object?>(null, jsonBytesResult.ErrorMessage);
        
        var jsonStream = new MemoryStream(jsonBytesResult.Value!);
        var deserializationResult = await TryDeserializeObjectAsync(jsonStream,targetType);
        if (!deserializationResult.IsSuccess) return new Result<object?>(null, deserializationResult.ErrorMessage);

        if (deserializationResult.Value is null) return new Result<object?>(null, "Error while deserializing");

        return deserializationResult with { ErrorMessage = null };
    }
}