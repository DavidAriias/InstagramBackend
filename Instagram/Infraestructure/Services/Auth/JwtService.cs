﻿using Instagram.Domain.Repositories.Interfaces.Auth;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Instagram.Infraestructure.Services.Identity
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<JwtService> _logger;
        public JwtService(IConfiguration configuration, ILogger<JwtService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public string GenerateAccessToken(string userId)
        {
            // Lógica de generación del token de acceso
            var accessClaims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            };
            //Cambiar a 60 mins
            return GenerateToken(accessClaims, TimeSpan.FromMinutes(60));  // Retorna el token de acceso
        }

        public string GenerateRefreshToken(string userId)
        {
            // Lógica de generación del token de actualización
            var refreshClaims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim("isRefreshToken", "true")
            };

            //Cambiar a 30 dias
            return GenerateToken(refreshClaims, TimeSpan.FromDays(30));  // Retorna el token de actualización
        }

        private string GenerateToken(IEnumerable<Claim> claims, TimeSpan expiration)
        {
            var secretKey = _configuration["Jwt:SecretKey"];
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secretKey!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.Add(expiration),  // Fecha de expiración del token
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public bool IsTokenValid(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = GetValidationParameters(); // Define tus parámetros de validación aquí

            try
            {
                tokenHandler.ValidateToken(token, validationParameters, out SecurityToken securityToken);

                // Verificar si el token no está expirado
                var isValidLifetime = IsTokenValidLifetime(securityToken);

                return isValidLifetime;
            }
            catch (SecurityTokenException)
            {
                return false; // Token no válido
            }
        }

        public bool IsRefreshToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = GetValidationParameters();

            try
            {

                var claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken securityToken);

                // Verificar si la reclamación "isRefreshToken" existe y su valor es "true"
                var isRefreshTokenClaim = claimsPrincipal.HasClaim(c => c.Type == "isRefreshToken" && c.Value == "true");

                return true;

            } catch (SecurityTokenException)
            {
                return false;
            }
        }

        private static bool IsTokenValidLifetime(SecurityToken securityToken)
        {
            // Verificar si el token no está expirado
            return securityToken.ValidTo > DateTime.UtcNow;
        }

        // Método auxiliar para obtener los parámetros de validación
        private TokenValidationParameters GetValidationParameters()
        {
            var secretKey = _configuration["Jwt:SecretKey"];
            // Define los parámetros de validación según tus necesidades
            return new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true, // Verificar si el token ha caducado
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(secretKey!)), // Clave secreta
                ClockSkew = TimeSpan.Zero // Sin margen de tiempo adicional
            };
        }


    }
}

