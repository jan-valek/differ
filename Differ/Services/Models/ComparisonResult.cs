namespace Differ.Services.Models;

public class ComparisonResult
{
    public ComparisonStatus Status { get; set; }
    public List<DifferenceBlock>? Differences { get; set; }
}