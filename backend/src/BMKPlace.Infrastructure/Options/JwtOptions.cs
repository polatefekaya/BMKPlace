using System;

namespace BMKPlace.Infrastructure.Options;

public sealed class JwtOptions
{
    public required string Secret {get; init;}
    public required string Issuer {get; init;}
    public required string Audience {get; init;}
    public required string ExpiryMinutes {get; init;}
}
