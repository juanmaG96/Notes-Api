using System.ComponentModel.DataAnnotations;

namespace Notes.Api.DTOs;

public class CreateNoteRequestDto
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Type { get; set; } = "Idea";

    [Required]
    [MaxLength(50)]
    public string Color { get; set; } = "indigo";

    public DateTime? TargetDateUtc { get; set; }
}
