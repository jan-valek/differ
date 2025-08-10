using System.ComponentModel.DataAnnotations;
using Differ.Models;
using Microsoft.AspNetCore.Mvc;

namespace Differ.Controllers;

[ApiController]
[Route("v1/diff")]
public class JsonDiffController:ControllerBase
{
    [HttpPost("{id}/left")]
    public async Task<IActionResult> Left(
        [Required] [FromRoute] string id,
        [Required] [FromBody] InputType input)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
           
        return Ok();
    }
    
    [HttpPost("{id}/right")]
    public async Task<IActionResult> Right(
        [Required] [FromRoute] string id,
        [Required] [FromBody] InputType input)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        return Ok();
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> Diff([Required] string id)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        return Ok();
    }
}