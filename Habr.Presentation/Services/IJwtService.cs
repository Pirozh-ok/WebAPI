using Habr.Common.DTOs.UserDTOs;

namespace Habr.Presentation.Services
{
    public interface IJwtService
    {
        public string GenerateAccessToken(UserDTO user);
    }
}
