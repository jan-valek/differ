using Differ.Services.Models;

namespace Differ.Services;

/// <summary>
/// String comparer for comparing two strings.
/// </summary>
public interface IDiffStringComparer
{
    /// <summary>
    /// Compare two strings and return status with difference blocks.
    /// </summary>
    ComparisonResult Compare(string inputA, string inputB);
}