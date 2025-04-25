using System;
using BMKPlace.Application.Contracts.DTOs.Canvas;
using BMKPlace.Application.Contracts.DTOs.Common;
using BMKPlace.Application.Contracts.DTOs.Pixels;

namespace BMKPlace.Application.Contracts.Abstractions.Realtime;

public interface IPixelHubClient
{
    Task ReceivePixelUpdate(PixelDto pixelDto);
    Task ReceiveInitialCanvasState(CanvasStateDto canvasStateDto);
    Task ReceiveError(ErrorDto errorDto);
}
