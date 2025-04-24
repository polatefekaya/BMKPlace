using System;
using SharedKernel.Exceptions;

namespace BMKPlace.Domain.Exceptions;

public class UserCooldownException : DomainValidationException
{
    public int UserId { get; }
    public int CanvasId { get; }
    public DateTimeOffset CooldownEndTime { get; }

    public UserCooldownException(int userId, int canvasId, DateTimeOffset cooldownEndTime)
        : base($"User {userId} on canvas {canvasId} is on cooldown until {cooldownEndTime:o}.")
    {
        UserId = userId;
        CanvasId = canvasId;
        CooldownEndTime = cooldownEndTime;
    }
}
