using System.Security.Claims;

namespace Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(string userId, string username, IEnumerable<Claim>? additionalClaims = null);
}