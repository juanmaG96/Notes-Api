using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notes.Api.DTOs;
using Notes.Api.Services;

namespace Notes.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private const string CookieName = "notes_token";
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var (success, errorMessage, response) = await _authService.RegisterAsync(request);
        if (!success || response is null)
        {
            return BadRequest(new { message = errorMessage });
        }

        SetAuthCookie(response.Token);
        return CreatedAtAction(nameof(GetMe), new { id = response.User.Id }, response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var (success, errorMessage, response) = await _authService.LoginAsync(request);
        if (!success || response is null)
        {
            return Unauthorized(new { message = errorMessage });
        }

        SetAuthCookie(response.Token);
        return Ok(response);
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete(CookieName, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax
        });

        return NoContent();
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Token de autenticacion invalido." });
        }

        var user = await _authService.GetUserByIdAsync(userId);
        if (user is null)
        {
            return NotFound(new { message = "Usuario no encontrado." });
        }

        return Ok(user);
    }

    private void SetAuthCookie(string token)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddDays(7),
            IsEssential = true
        };

        Response.Cookies.Append(CookieName, token, cookieOptions);
    }
}
