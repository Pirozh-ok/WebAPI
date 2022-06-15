namespace Habr.Presentation
{
    public class JwtOptions
    {
        public const string Jwt = "Jwt";
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public string ExpiresInSecond { get; set; } = string.Empty;
    }
}
