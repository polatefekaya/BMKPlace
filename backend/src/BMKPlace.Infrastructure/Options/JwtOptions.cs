using System;

namespace BMKPlace.Infrastructure.Options;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";
    public string Secret {get; init;} = string.Empty;
    public string Issuer {get; init;} = string.Empty;
    public string Audience {get; init;} = string.Empty;
    public string ExpiryMinutes {get; init;} = string.Empty;
}
