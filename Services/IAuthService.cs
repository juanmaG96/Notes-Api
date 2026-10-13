using Notes.Api.DTOs;

namespace Notes.Api.Services;

public interface IAuthService
{
    Task<(bool Success, string? ErrorMessage, AuthResponseDto? Response)> RegisterAsync(RegisterRequestDto request);
    Task<(bool Success, string? ErrorMessage, AuthResponseDto? Response)> LoginAsync(LoginRequestDto request);
    Task<UserDto?> GetUserByIdAsync(Guid userId);
}
