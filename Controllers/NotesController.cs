using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notes.Api.DTOs;
using Notes.Api.Services;

namespace Notes.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class NotesController : ControllerBase
{
    private readonly INoteService _noteService;

    public NotesController(INoteService noteService)
    {
        _noteService = noteService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? type,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized(new { message = "Token de autenticacion invalido." });
        }

        var notes = await _noteService.GetNotesByUserAsync(userId.Value, type, startDate, endDate);
        return Ok(notes);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized(new { message = "Token de autenticacion invalido." });
        }

        var note = await _noteService.GetNoteByIdAsync(id, userId.Value);
        if (note is null)
        {
            return NotFound(new { message = "Nota o meta no encontrada." });
        }

        return Ok(note);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateNoteRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized(new { message = "Token de autenticacion invalido." });
        }

        var created = await _noteService.CreateNoteAsync(userId.Value, request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateNoteRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized(new { message = "Token de autenticacion invalido." });
        }

        var updated = await _noteService.UpdateNoteAsync(id, userId.Value, request);
        if (updated is null)
        {
            return NotFound(new { message = "Nota o meta no encontrada." });
        }

        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized(new { message = "Token de autenticacion invalido." });
        }

        var deleted = await _noteService.DeleteNoteAsync(id, userId.Value);
        if (!deleted)
        {
            return NotFound(new { message = "Nota o meta no encontrada." });
        }

        return NoContent();
    }

    private Guid? GetUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return null;
        }

        return userId;
    }
}
