using Differ.Data;
using Differ.Exceptions;
using Differ.Models;
using Microsoft.EntityFrameworkCore;

namespace Differ.Repositories;

public class DiffRepository(DiffContext dbContext, ILogger<DiffRepository> logger) : IDiffRepository
{
    /// <inheritdoc />
    public async Task StoreInputAsync(string id, Side side, string inputValue, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));
        if (string.IsNullOrEmpty(inputValue)) throw new ArgumentNullException(nameof(inputValue));

        // Optimistic concurency, first we tried to update data, and if they don't exist we do insert.
        var rowsAffected = await dbContext.DiffEntities
            .Where(e => e.Key == id)
            .ExecuteUpdateAsync(setters => 
                side == Side.Right 
                    ? setters.SetProperty(e => e.Right, inputValue) 
                    : setters.SetProperty(e => e.Left, inputValue), ct);

        if (rowsAffected > 0)
            return;

        var entity = new DiffEntity
        {
            Key = id,
            Right = side == Side.Right ? inputValue : null,
            Left = side == Side.Left ? inputValue : null,
        };

        dbContext.DiffEntities.Add(entity);
        
        try
        {
            await dbContext.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex,"Inserting of the new entity failed. Entity may exists.");
            dbContext.ChangeTracker.Clear();
            throw new DuplicateKeyException(id);
        }
    }

    /// <inheritdoc />
    public async Task<DiffEntity?> ReadInputAsync(string id, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));
        return await dbContext.DiffEntities
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Key == id, ct);
    }
}