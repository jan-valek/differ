using System.Diagnostics.CodeAnalysis;
using Differ.Services.Models;

namespace Differ.Services;

public class DiffStringComparer : IDiffStringComparer
{
    /// <summary>
    /// Compare two strings and return status.
    /// </summary>
    public ComparisonResult Compare(string inputA, string inputB)
    {
        if (string.IsNullOrEmpty(inputA)) inputA = string.Empty;
        if (string.IsNullOrEmpty(inputB)) inputB = string.Empty;

        if (inputA.Length != inputB.Length) return CreateResult(ComparisonStatus.DifferentSize);

        if (string.CompareOrdinal(inputA, inputB) == 0) return CreateResult(ComparisonStatus.Equal);

        var differences = CalculateDifferenceBlocks(inputA, inputB);

        return new ComparisonResult
        {
            Status = ComparisonStatus.SameSizeNotEqual,
            Differences = differences
        };
    }

    /// <summary>
    /// Internal algorithm for calculating difference blogs.
    /// </summary>
    private static List<DifferenceBlock> CalculateDifferenceBlocks(string inputA, string inputB)
    {
        var differences = new List<DifferenceBlock>();
        int? diffStart = null;

        for (var i = 0; i <= inputA.Length; i++)
        {
            var isDifferent = i < inputA.Length && inputA[i] != inputB[i];

            switch (isDifferent)
            {
                case true when diffStart == null:
                    diffStart = i;
                    break;
                case false when diffStart != null:
                    AddNewDiffBlock(differences, diffStart, i);
                    diffStart = null;
                    break;
            }
        }

        return differences;
    }

    private static void AddNewDiffBlock(List<DifferenceBlock> differences, [DisallowNull] int? diffStart, int i)
    {
        differences.Add(new DifferenceBlock
        {
            Offset = diffStart.Value,
            Length = i - diffStart.Value
        });
    }

    private ComparisonResult CreateResult(ComparisonStatus status)
    {
        return new ComparisonResult { Status = status };
    }
}