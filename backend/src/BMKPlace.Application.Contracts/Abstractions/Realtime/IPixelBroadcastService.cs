using System;
using BMKPlace.Application.Contracts.DTOs.Pixels;

namespace BMKPlace.Application.Contracts.Abstractions.Realtime;

public interface IPixelBroadcastService
{
    Task BroadcastPixelUpdateAsync(PixelDto pixelDto, CancellationToken cancellationToken = default);
}
