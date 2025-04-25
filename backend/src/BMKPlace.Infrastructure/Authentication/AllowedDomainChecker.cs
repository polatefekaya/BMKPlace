using System;
using BMKPlace.Application.Contracts.Abstractions.Authentication;
using BMKPlace.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BMKPlace.Infrastructure.Authentication;

public class AllowedDomainChecker : IAllowedDomainChecker
{
    private readonly AllowedDomainOptions _allowedDomainOptions;
    private readonly ILogger<AllowedDomainChecker> _logger;
    private readonly HashSet<string> _allowedDomains; // Cache the parsed domains

    public AllowedDomainChecker(IOptions<AllowedDomainOptions> options, ILogger<AllowedDomainChecker> logger)
    {
        _allowedDomainOptions = options.Value;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Load and parse allowed domains from configuration on startup
        if (string.IsNullOrWhiteSpace(_allowedDomainOptions.AllowedDomains))
        {
            _logger.LogWarning("Allowed domains configuration ('Authentication:AllowedDomains') is not set. No domains will be allowed.");
            _allowedDomains = new HashSet<string>(StringComparer.OrdinalIgnoreCase); // Empty set
        }
        else
        {
            _allowedDomains = _allowedDomainOptions.AllowedDomains.Split([ ',', ';', ' ' ], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                                              .Select(d => d.ToLowerInvariant()) // Normalize to lower case
                                              .ToHashSet(StringComparer.OrdinalIgnoreCase); // Use case-insensitive comparison
            _logger.LogInformation("Loaded {DomainCount} allowed domains from configuration.", _allowedDomains.Count);
        }
    }

    public Task<bool> IsDomainAllowedAsync(string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return Task.FromResult(false);
        }

        try
        {
            // Very basic domain extraction - might need refinement for edge cases
            int atIndex = email.LastIndexOf('@');
            if (atIndex < 0 || atIndex == email.Length - 1)
            {
                _logger.LogWarning("Could not extract domain from invalid email format: {Email}", email);
                return Task.FromResult(false); // Invalid email format
            }

            string domain = email.Substring(atIndex + 1).ToLowerInvariant().Trim(); // Normalize domain

            bool isAllowed = _allowedDomains.Contains(domain);

            if (isAllowed)
            {
                _logger.LogDebug("Domain '{Domain}' from email {Email} is allowed.", domain, email);
            }
            else
            {
                _logger.LogWarning("Domain '{Domain}' from email {Email} is NOT in the allowed list.", domain, email);
            }

            return Task.FromResult(isAllowed);
        }
        catch (Exception ex)
        {
            // Log error during parsing
            _logger.LogError(ex, "Error checking domain allowance for email {Email}", email);
            return Task.FromResult(false); // Treat errors as not allowed
        }
    }
}
