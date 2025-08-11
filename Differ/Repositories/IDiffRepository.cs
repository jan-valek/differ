using Differ.Data;
using Differ.Models;

namespace Differ.Repositories;

public interface IDiffRepository
{
    /// <summary>
    /// Upsert data to db.
    /// </summary>
    Task StoreInputAsync(string id, Side side, string inputValue, CancellationToken ct = default);
    /// <summary>
    /// Read data from the db.
    /// </summary>
    Task<DiffEntity?> ReadInputAsync(string id, CancellationToken ct = default);
}