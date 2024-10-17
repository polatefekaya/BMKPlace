using System;

namespace DbServer.Domain.Data.Models.Errors;

public class DatabaseModel
{
    public bool IsError { get; }
    public string? ErrorMessage { get; }
    public int Code { get; }

    private DatabaseModel (bool isError, string? errorMessage, int code){
        IsError = isError;
        ErrorMessage = errorMessage;
        Code = code;
    }

    public static DatabaseModel Success() => new DatabaseModel(false, null, 200);
    public static DatabaseModel Error(string message, int code) => new DatabaseModel(true, message, code);
}
