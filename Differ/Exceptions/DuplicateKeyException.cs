namespace Differ.Exceptions;

public class DuplicateKeyException : Exception
{
    public DuplicateKeyException(string key) 
        : base($"Entity with key '{key}' already exists.")
    {
        Key = key;
    }
    
    public string Key { get; }
}