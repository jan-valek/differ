using System.Text;

namespace Differ.CustomBinders.Deserializer;

internal interface IBase64JsonDeserializer
{
    Task<Result<Object?>> DeserializeAsync(Stream stream, Type targetType, Encoding encoding);
}