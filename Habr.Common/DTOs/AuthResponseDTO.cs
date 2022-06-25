namespace Habr.Common.DTOs
{
    internal class AuthResponseDTO
    {
        public string UserName;
        public Roles Role;
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiration { get; set; }
    }
}
