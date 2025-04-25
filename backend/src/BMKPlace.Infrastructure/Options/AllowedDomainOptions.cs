using System;

namespace BMKPlace.Infrastructure.Options;

public class AllowedDomainOptions
{
    public const string SectionName = "Authentication";
    public required string AllowedDomains {get; init;}
}
