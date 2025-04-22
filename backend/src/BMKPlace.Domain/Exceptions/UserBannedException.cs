using System;
using SharedKernel.Exceptions;

namespace BMKPlace.Domain.Exceptions;

public class UserBannedException : DomainValidationException
{
    public long UserId { get; }
    public int CanvasId { get; }

    public UserBannedException(long userId, int canvasId)
        : base($"User {userId} is banned from performing actions on canvas {canvasId}.")
    {
        UserId = userId;
        CanvasId = canvasId;
    }
}
