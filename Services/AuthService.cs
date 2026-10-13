using Microsoft.EntityFrameworkCore;
using Notes.Api.Data;
using Notes.Api.DTOs;
using Notes.Api.Models;

namespace Notes.Api.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public AuthService(
        AppDbContext dbContext,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<(bool Success, string? ErrorMessage, AuthResponseDto? Response)> RegisterAsync(RegisterRequestDto request)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var existingUser = await _dbContext.Users
            .AnyAsync(u => u.Email == normalizedEmail);

        if (existingUser)
        {
            return (false, "El correo electronico ya se encuentra registrado.", null);
        }

        var passwordHash = _passwordHasher.HashPassword(request.Password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = normalizedEmail,
            PasswordHash = passwordHash,
            CreatedAtUtc = DateTime.UtcNow,
            LastLoginAtUtc = null
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        var token = _tokenService.GenerateToken(user);

        var responseDto = new AuthResponseDto
        {
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                CreatedAtUtc = user.CreatedAtUtc
            },
            Token = token
        };

        return (true, null, responseDto);
    }

    public async Task<(bool Success, string? ErrorMessage, AuthResponseDto? Response)> LoginAsync(LoginRequestDto request)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == normalizedEmail);

        if (user is null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            return (false, "Credenciales invalidas.", null);
        }

        user.LastLoginAtUtc = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        var token = _tokenService.GenerateToken(user);

        var responseDto = new AuthResponseDto
        {
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                CreatedAtUtc = user.CreatedAtUtc
            },
            Token = token
        };

        return (true, null, responseDto);
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid userId)
    {
        var user = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
        {
            return null;
        }

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            CreatedAtUtc = user.CreatedAtUtc
        };
    }
}
