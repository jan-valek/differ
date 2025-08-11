using System.ComponentModel.DataAnnotations;
using Differ.Exceptions;
using Differ.Models;
using Differ.Repositories;
using Differ.Services;
using Differ.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace Differ.Controllers;

[ApiController]
[Route("v1/diff")]
public class JsonDiffController(
    IDiffRepository repository,
    IDiffStringComparer comparer,
    ILogger<JsonDiffController> logger) : ControllerBase
{
    [HttpPost("{id}/left")]
    public async Task<IActionResult> Left(
        [Required] [FromRoute] string id,
        [Required] [FromBody] InputType input)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            await repository.StoreInputAsync(id, Side.Left, input.Input);
            return Created();
        }
        catch (DuplicateKeyException)
        {
            logger.LogWarning("Duplicate key attempt for id {Id} on left side.", id);
            return Conflict(new { error = "Entity with this key already exists." });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error storing left input for id {Id}.", id);
            return StatusCode(500, new { error = "Internal server error." });
        }
    }

    [HttpPost("{id}/right")]
    public async Task<IActionResult> Right(
        [Required] [FromRoute] string id,
        [Required] [FromBody] InputType input)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            await repository.StoreInputAsync(id, Side.Right, input.Input);
            return Created();
        }
        catch (DuplicateKeyException)
        {
            logger.LogWarning("Duplicate key attempt for id {Id} on right side.", id);
            return Conflict(new { error = "Entity with this key already exists." });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error storing right input for id {Id}.", id);
            return StatusCode(500, new { error = "Internal server error." });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Diff([Required] string id)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var entity = await repository.ReadInputAsync(id);

            if (entity == null)
            {
                return NotFound(new { error = "No data found for this ID." });
            }

            if (entity.Left == null || entity.Right == null)
            {
                return BadRequest(new { error = "Both left and right inputs must be provided before comparison." });
            }

            var result = comparer.Compare(entity.Left, entity.Right);

            var response = result.Status switch
            {
                ComparisonStatus.Equal => new DiffResult("Inputs were equal.", null),
                ComparisonStatus.DifferentSize => new DiffResult("Inputs are of different size.", null),
                ComparisonStatus.SameSizeNotEqual => new DiffResult("Inputs have same size but are different.", result.Differences),
                _ => throw new InvalidOperationException("Unknown comparison status.")
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error comparing inputs for id {Id}.", id);
            return StatusCode(500, new { error = "Internal server error." });
        }
    }
}