using Habr.Common.DTOs.UserDTOs;

namespace Habr.Common.DTOs
{
    public class AuthResponseDTO
    {
        public UserDTO UserData { get; set; }
        public string AccessToken { get; set; }
    }
}
