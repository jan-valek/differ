namespace Differ.Data;

public class DiffEntity
{
    public int Id { get; set; }
    public required string Key { get; set; }
    public string? Left { get; set; }
    public string? Right { get; set; }
}