using Habr.DataAccess;

namespace Habr.Common.DTOs.UserDTOs
{
    public class IdentityDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpirationDate { get; set; }
        public Roles Role { get; set; }
    }
}
