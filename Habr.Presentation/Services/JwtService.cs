using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Habr.Common.DTOs.UserDTOs;
using Microsoft.Extensions.Options;
using Habr.Presentation.Auth;
using System.Security.Cryptography;
using Habr.DataAccess;
using Microsoft.EntityFrameworkCore;
using Habr.Common.DTOs;
using Habr.Common.Exceptions;
using Habr.Common.Resources;

namespace Habr.Presentation.Services
{
    public class JwtService : IJwtService
    {
        private readonly JwtOptions _options;
        private readonly IConfiguration _configuration;
        private readonly DataContext _context; 

        public JwtService(IOptions<JwtOptions> options, IConfiguration configuration, DataContext context)
        {
            _options = options.Value;
            _configuration = configuration;
            _context = context;
        }

        public string GenerateAccessToken(IdentityDTO user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString())
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

        public RefreshToken GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var generator = new RNGCryptoServiceProvider();
            generator.GetBytes(randomNumber);

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                Expires = DateTime.UtcNow.AddDays(int.Parse(_configuration["RefreshToken:RefreshTokenValidityInDays"])),
                Created = DateTime.UtcNow
            };
        }

        public void SetRefreshTokenInCookie(string refreshToken, HttpResponse response)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(int.Parse(_configuration["RefreshToken:RefreshTokenValidityInDays"]))
            };

            response.Cookies.Append("RefreshToken", refreshToken, cookieOptions);
        }

        public async Task<RefreshToken> UpdateRefreshTokenUserAsync(int userId)
        {
            var refreshToken = GenerateRefreshToken();
            var user = await _context.Users
                .SingleOrDefaultAsync(u => u.Id == userId);

            user.RefreshToken = refreshToken.Token;
            user.RefreshTokenExpirationDate = refreshToken.Expires;

            _context.Update(user);
            await _context.SaveChangesAsync();

            return refreshToken; 
        }

        public async Task<AuthResponseDTO> RefreshJwtToken(string refreshToken, HttpResponse response)
        {
            var user = await _context.Users
                .SingleOrDefaultAsync(u => u.RefreshToken == refreshToken);

            if (user is null)
            {
                throw new BusinessException(TokenExceptionMessageResource.NoCorrectRefreshToken);
            }

            if (user.RefreshTokenExpirationDate < DateTime.UtcNow)
            {
                throw new BusinessException(TokenExceptionMessageResource.TokenNotActive);
            }

            var newRefreshToken = await UpdateRefreshTokenUserAsync(user.Id);
            SetRefreshTokenInCookie(newRefreshToken.Token, response);
            var accessToken = GenerateAccessToken(
                new IdentityDTO
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    RegistrationDate = user.RegistrationDate,
                });

            return new AuthResponseDTO
            {
                AccessToken = accessToken,
                UserData = new UserDTO
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    RegistrationDate = user.RegistrationDate,
                }
            };
        }
    }
}
