using System.Diagnostics.CodeAnalysis;
using Differ.Services.Models;

namespace Differ.Services;

public class DiffStringComparer : IDiffStringComparer
{
    public ComparisonResult Compare(string a, string b)
    {
        if (a == null) a = string.Empty;
        if (b == null) b = string.Empty;
        
        if (a.Length != b.Length) return CreateResult(ComparisonStatus.DifferentSize);
        
        if (string.CompareOrdinal(a,b)==0) return CreateResult(ComparisonStatus.Equal);
        
        var differences = CalculateDifferenceBlocks(a, b);

        return new ComparisonResult
        {
            Status = ComparisonStatus.SameSizeNotEqual,
            Differences = differences
        };
    }

    private static List<DifferenceBlock> CalculateDifferenceBlocks(string a, string b)
    {
        var differences = new List<DifferenceBlock>();
        int? diffStart = null;
        
        for (var i = 0; i <= a.Length; i++)
        {
            var isDifferent = i < a.Length && a[i] != b[i];

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
        differences.Add(new DifferenceBlock {
            Offset = diffStart.Value,
            Length = i - diffStart.Value
        });
    }

    private ComparisonResult CreateResult(ComparisonStatus status)
    {
        return new ComparisonResult { Status = status };
    }
}