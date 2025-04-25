using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BMKPlace.Application.Contracts.Abstractions.Authentication;
using BMKPlace.Infrastructure.Common.Exceptions;
using BMKPlace.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BMKPlace.Infrastructure.Authentication;

public class TokenService : ITokenService
{
    private readonly JwtOptions _jwtOptions;

    public TokenService(IOptionsSnapshot<JwtOptions> jwtOptionsSnapshot){
        _jwtOptions = jwtOptionsSnapshot.Value;
    }

    public Task<string> GenerateTokenAsync(
        int userId,
        string username,
        IEnumerable<string>? roles = null,
        Dictionary<string, string>? additionalClaims = null)
    {
        if (string.IsNullOrEmpty(_jwtOptions.Secret) || string.IsNullOrEmpty(_jwtOptions.Issuer) || string.IsNullOrEmpty(_jwtOptions.Audience) || string.IsNullOrEmpty(_jwtOptions.ExpiryMinutes))
        {
            // Log Error: JWT Configuration is missing or incomplete.
            // Consider throwing a specific configuration exception or returning null/error indicator.
            // Throwing for now as it's critical for operation.
            throw new JwtConfigurationMissingException("JWT configuration (Secret, Issuer, Audience, ExpiryMinutes) is missing or incomplete in application settings.");
        }

        if (!int.TryParse(_jwtOptions.ExpiryMinutes, out int expiryMinutes) || expiryMinutes <= 0)
        {
            throw new InvalidOperationException("Invalid JWT ExpiryMinutes configuration value.");
        }

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()), 
            new Claim(JwtRegisteredClaimNames.Name, username),  
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), 
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        if (roles != null)
        {
            foreach (string role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
        }

        if (additionalClaims != null)
        {
            foreach (KeyValuePair<string, string> claimPair in additionalClaims)
            {
                claims.Add(new Claim(claimPair.Key, claimPair.Value));
            }
        }

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(expiryMinutes),
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);
        string token = tokenHandler.WriteToken(securityToken);

        return Task.FromResult(token);
    }
}
