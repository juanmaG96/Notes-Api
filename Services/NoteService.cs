using Microsoft.EntityFrameworkCore;
using Notes.Api.Data;
using Notes.Api.DTOs;
using Notes.Api.Models;

namespace Notes.Api.Services;

public class NoteService : INoteService
{
    private readonly AppDbContext _dbContext;

    public NoteService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<NoteResponseDto>> GetNotesByUserAsync(
        Guid userId, 
        string? type = null, 
        DateTime? startDate = null, 
        DateTime? endDate = null)
    {
        var query = _dbContext.Notes
            .AsNoTracking()
            .Where(n => n.UserId == userId);

        if (!string.IsNullOrWhiteSpace(type))
        {
            var normalizedType = type.Trim().ToLower();
            query = query.Where(n => n.Type.ToLower() == normalizedType);
        }

        if (startDate.HasValue)
        {
            query = query.Where(n => n.TargetDateUtc.HasValue && n.TargetDateUtc.Value >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(n => n.TargetDateUtc.HasValue && n.TargetDateUtc.Value <= endDate.Value);
        }

        return await query
            .OrderByDescending(n => n.CreatedAtUtc)
            .Select(n => new NoteResponseDto
            {
                Id = n.Id,
                Title = n.Title,
                Content = n.Content,
                Type = n.Type,
                Color = n.Color,
                CreatedAtUtc = n.CreatedAtUtc,
                UpdatedAtUtc = n.UpdatedAtUtc,
                TargetDateUtc = n.TargetDateUtc
            })
            .ToListAsync();
    }

    public async Task<NoteResponseDto?> GetNoteByIdAsync(Guid noteId, Guid userId)
    {
        var note = await _dbContext.Notes
            .AsNoTracking()
            .FirstOrDefaultAsync(n => n.Id == noteId && n.UserId == userId);

        if (note is null)
        {
            return null;
        }

        return new NoteResponseDto
        {
            Id = note.Id,
            Title = note.Title,
            Content = note.Content,
            Type = note.Type,
            Color = note.Color,
            CreatedAtUtc = note.CreatedAtUtc,
            UpdatedAtUtc = note.UpdatedAtUtc,
            TargetDateUtc = note.TargetDateUtc
        };
    }

    public async Task<NoteResponseDto> CreateNoteAsync(Guid userId, CreateNoteRequestDto request)
    {
        var note = new Note
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = request.Title.Trim(),
            Content = request.Content,
            Type = string.IsNullOrWhiteSpace(request.Type) ? "Idea" : request.Type.Trim(),
            Color = string.IsNullOrWhiteSpace(request.Color) ? "indigo" : request.Color.Trim().ToLowerInvariant(),
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = null,
            TargetDateUtc = request.TargetDateUtc
        };

        _dbContext.Notes.Add(note);
        await _dbContext.SaveChangesAsync();

        return new NoteResponseDto
        {
            Id = note.Id,
            Title = note.Title,
            Content = note.Content,
            Type = note.Type,
            Color = note.Color,
            CreatedAtUtc = note.CreatedAtUtc,
            UpdatedAtUtc = note.UpdatedAtUtc,
            TargetDateUtc = note.TargetDateUtc
        };
    }

    public async Task<NoteResponseDto?> UpdateNoteAsync(Guid noteId, Guid userId, UpdateNoteRequestDto request)
    {
        var note = await _dbContext.Notes
            .FirstOrDefaultAsync(n => n.Id == noteId && n.UserId == userId);

        if (note is null)
        {
            return null;
        }

        note.Title = request.Title.Trim();
        note.Content = request.Content;
        note.Type = string.IsNullOrWhiteSpace(request.Type) ? "Idea" : request.Type.Trim();
        note.Color = string.IsNullOrWhiteSpace(request.Color) ? "indigo" : request.Color.Trim().ToLowerInvariant();
        note.TargetDateUtc = request.TargetDateUtc;
        note.UpdatedAtUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        return new NoteResponseDto
        {
            Id = note.Id,
            Title = note.Title,
            Content = note.Content,
            Type = note.Type,
            Color = note.Color,
            CreatedAtUtc = note.CreatedAtUtc,
            UpdatedAtUtc = note.UpdatedAtUtc,
            TargetDateUtc = note.TargetDateUtc
        };
    }

    public async Task<bool> DeleteNoteAsync(Guid noteId, Guid userId)
    {
        var note = await _dbContext.Notes
            .FirstOrDefaultAsync(n => n.Id == noteId && n.UserId == userId);

        if (note is null)
        {
            return false;
        }

        _dbContext.Notes.Remove(note);
        await _dbContext.SaveChangesAsync();

        return true;
    }
}
