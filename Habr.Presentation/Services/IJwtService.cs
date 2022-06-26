using Habr.Common.DTOs.UserDTOs;
using Habr.Presentation.Auth;

namespace Habr.Presentation.Services
{
    public interface IJwtService
    {
        public string GenerateAccessToken(RegistrationOrLoginUserDTO user);
        public RefreshToken GenerateRefreshToken();
        public void SetRefreshTokenInCookie(string refreshToken, HttpResponse response);
        public Task<RefreshToken> UpdateRefreshTokenUser(int userId);
    }
}
