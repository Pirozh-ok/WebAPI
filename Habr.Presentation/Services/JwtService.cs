using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Habr.Common.DTOs.UserDTOs;
using Microsoft.Extensions.Options;
using Habr.Presentation.Auth;

namespace Habr.Presentation.Services
{
    public class JwtService : IJwtService
    {
        private readonly JwtOptions _options;

        public JwtService(IOptions<JwtOptions> options)
        {
            _options = options.Value;
        }

        public string GenerateAccessToken(UserDTO user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, user.Id.ToString())
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));

            var tokenDescription = new SecurityTokenDescriptor
            {
                Issuer = _options.Issuer,
                Audience = _options.Audience,
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddSeconds(int.Parse(_options.TokenValidityInSecond)),
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescription);
            return tokenHandler.WriteToken(token);
        }
    }
}
