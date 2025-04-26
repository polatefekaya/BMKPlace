using Mediator;

namespace BMKPlace.Application.Features.PixelCanvas.Commands.ActivateCanvas;

public sealed record ActivateCanvasCommand(int CanvasId) : ICommand;
