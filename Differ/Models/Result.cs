namespace Differ.Models;

/// <summary>
/// Represents the result of an operation, containing either a value or an error message.
/// </summary>
internal record Result<T>(T? Value, string? ErrorMessage)
{
    public bool IsSuccess => ErrorMessage is null;
}