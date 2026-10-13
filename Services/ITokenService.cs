using Notes.Api.Models;

namespace Notes.Api.Services;

public interface ITokenService
{
    string GenerateToken(User user);
}
