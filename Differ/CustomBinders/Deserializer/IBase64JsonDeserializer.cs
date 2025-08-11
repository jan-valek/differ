using System.Text;
using Differ.Models;

namespace Differ.CustomBinders.Deserializer;

internal interface IBase64JsonDeserializer
{
    /// <summary>
    /// Deserialize stream of data to result type.
    /// </summary>
    Task<Result<Object?>> DeserializeAsync(Stream stream, Type targetType, Encoding encoding);
}