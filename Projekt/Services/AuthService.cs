using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Projekt.Data;
using Projekt.DTOs.AuthDtos;
using Projekt.Entities.AuthModels;
using Projekt.Exceptions;
using Projekt.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Projekt.Services
{
    public class AuthService : IAuthService
    {

        private readonly AppDbContext _dbContext;
        private readonly IConfiguration _config;

        public AuthService(AppDbContext dbContext, IConfiguration config)
        {
            _dbContext = dbContext;
            _config = config;
        }

        public async Task<TokenDto> Login(LoginDto dto)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Login == dto.Login);
            if (user == null) throw new AccessDeniedException("Nieprawidlowy login lub haslo");

            var validPassword = BCrypt.Net.BCrypt.Verify(dto.Password, user.HashPassword);

            if (!validPassword) throw new AccessDeniedException("Nieprawidlowy login lub haslo");
            var token = GenerateToken(user);
            user.RefreshToken = token.RefreshToken;
            user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(1);

            await _dbContext.SaveChangesAsync();
            return token;
        }

        public async Task<TokenDto> Refresh(RefreshDto dto)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.RefreshToken == dto.RefreshToken);
            if (user == null) throw new AccessDeniedException("Nieprawidlowy refresh token");

            if (user.RefreshTokenExpiresAt < DateTime.UtcNow) throw new AccessDeniedException("Refresh token wygasl");

            var token = GenerateToken(user);
            user.RefreshToken = token.RefreshToken;
            user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(1);

            await _dbContext.SaveChangesAsync();
            return token;
        }

        private TokenDto GenerateToken(User user)
        {
            var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Login),
            new(ClaimTypes.Role, user.Role)
        };
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: creds);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwt);

            var refreshToken = Convert.ToBase64String(
                RandomNumberGenerator.GetBytes(64));

            return new TokenDto(accessToken, refreshToken);
        }
    }
}
