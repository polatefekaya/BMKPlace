using System;
using BMKPlace.Application.Contracts.DTOs.Pixels;

namespace BMKPlace.Application.Contracts.Abstractions.Realtime;

public interface IPixelNotifier
{
    Task NotifyPixelUpdateAsync(PixelDto pixelDto, CancellationToken cancellationToken = default);
}
