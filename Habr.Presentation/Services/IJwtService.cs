using Habr.Common.DTOs;
using Habr.Common.DTOs.UserDTOs;
using Habr.Presentation.Auth;

namespace Habr.Presentation.Services
{
    public interface IJwtService
    {
        public string GenerateAccessToken(IdentityDTO user);
        public RefreshToken GenerateRefreshToken();
        public void SetRefreshTokenInCookie(string refreshToken, HttpResponse response);
        public Task<RefreshToken> UpdateRefreshTokenUserAsync(int userId);
        public Task<AuthResponseDTO> RefreshJwtToken(string refreshToken, HttpResponse response);
    }
}
