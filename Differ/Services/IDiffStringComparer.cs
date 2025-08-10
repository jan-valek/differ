using Differ.Services.Models;

namespace Differ.Services;

public interface IDiffStringComparer
{
    ComparisonResult Compare(string a, string b);
}