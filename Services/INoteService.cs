using Notes.Api.DTOs;

namespace Notes.Api.Services;

public interface INoteService
{
    Task<IEnumerable<NoteResponseDto>> GetNotesByUserAsync(Guid userId, string? type = null, DateTime? startDate = null, DateTime? endDate = null);
    Task<NoteResponseDto?> GetNoteByIdAsync(Guid noteId, Guid userId);
    Task<NoteResponseDto> CreateNoteAsync(Guid userId, CreateNoteRequestDto request);
    Task<NoteResponseDto?> UpdateNoteAsync(Guid noteId, Guid userId, UpdateNoteRequestDto request);
    Task<bool> DeleteNoteAsync(Guid noteId, Guid userId);
}
