using System;
using BMKPlace.Application.Contracts.Abstractions.Authentication;
using BMKPlace.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace BMKPlace.Infrastructure.Authentication;

public class UserLookupService : IUserLookupService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<UserLookupService> _logger;

    public UserLookupService(UserManager<ApplicationUser> userManager, ILogger<UserLookupService> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<int?> FindUserIdByIdentifierAsync(string identifier, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(identifier);

        string normalizedEmail = identifier.ToUpperInvariant().Trim();
        _logger.LogDebug("Attempting to find user by normalized identifier (email): {NormalizedEmail}", normalizedEmail);

        ApplicationUser? user = await _userManager.FindByEmailAsync(identifier.Trim());

        if (user != null)
        {
            _logger.LogInformation("User found with ID {UserId} for identifier {Identifier}", user.Id, identifier);
            return user.Id; 
        }
        else
        {
            _logger.LogInformation("User not found for identifier {Identifier}", identifier);
            return null;
        }
    }
}
