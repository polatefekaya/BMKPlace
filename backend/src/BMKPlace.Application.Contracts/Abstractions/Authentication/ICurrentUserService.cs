using System;

namespace BMKPlace.Application.Contracts.Abstractions.Authentication;

public interface ICurrentUserService
{
    int? UserId { get; }
}
