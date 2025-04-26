using Mediator;

namespace BMKPlace.Application.Features.PixelCanvas.Commands.DeactivateCanvas;

public sealed record DeactivateCanvasCommand(int CanvasId) : ICommand;
