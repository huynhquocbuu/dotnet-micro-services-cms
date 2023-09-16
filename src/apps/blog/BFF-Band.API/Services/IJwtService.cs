using BFF_Band.API.Models;

namespace BFF_Band.API.Services;

public interface IJwtService
{
    void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
    bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
    string CreateToken(User user);
    RefreshToken GenerateRefreshToken();
}