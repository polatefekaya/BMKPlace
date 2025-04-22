using System;

namespace BMKPlace.Application.Contracts.Abstractions.Authentication;

public interface ITokenService
{
    Task<string> GenerateTokenAsync(
        int userId,
        string username,
        IEnumerable<string>? roles = null,
        Dictionary<string, string>? additionalClaims = null);
}
