namespace BMKPlace.Application.Contracts.DTOs.Common;

public record ErrorDto( string Type, string Title, int Status, string Detail, string? Instance = null );
