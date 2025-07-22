using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using RpgGame.Application.DTOs.Authentication;

namespace RpgGame.Application.Interfaces.Authentication
{
    public interface IJwtTokenService
    {
        string GenerateAccessToken(UserDto user);
        string GenerateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
        Task<bool> ValidateTokenAsync(string token);
        Task<bool> ValidateRefreshTokenAsync(string refreshToken, string userId);
        Task StoreRefreshTokenAsync(string refreshToken, string userId, DateTime expiresAt);
        Task RevokeRefreshTokenAsync(string refreshToken);
    }
}