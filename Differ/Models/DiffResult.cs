using Differ.Services.Models;

namespace Differ.Models;

public record DiffResult(string Status, List<DifferenceBlock>? Differences);