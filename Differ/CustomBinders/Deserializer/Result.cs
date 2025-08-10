namespace Differ.CustomBinders.Deserializer;

internal record Result<T>(T? Value, string? ErrorMessage)
{
    public bool IsSuccess => ErrorMessage is null;
}