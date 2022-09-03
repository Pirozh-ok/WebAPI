using Microsoft.AspNetCore.Http;

namespace Habr.Common.DTOs.UserDTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string Avatar { get; set; }
    }
}
