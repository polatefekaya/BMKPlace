namespace BMKPlace.Application.Contracts.DTOs.Authentication;

public record AuthenticationResponse( string AccessToken, int UserId, string Username );
