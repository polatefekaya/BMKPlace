using System;
using System.Reflection.Metadata.Ecma335;
using DbServer.Domain.Data.Models.Errors;

namespace DbServer.Domain.Data.Results;

public class DatabaseResult<T>
{
    public T? Data { get; }
    public DatabaseModel Model { get; }
    public DateTime Timestamp { get; }
    public bool IsError => Model.IsError;
    public int RowsAffected { get; }

    public DatabaseResult(){}

    private DatabaseResult(T? data, DatabaseModel model, int rowsAffected = 0){
        Data = data;
        Model = model;
        Timestamp = DateTime.UtcNow;
        RowsAffected = rowsAffected;
    }

    public static DatabaseResult<T> Success(T? data = default, int rowsAffected = 1) => 
        new DatabaseResult<T>(
            data, 
            DatabaseModel.Success(),
            rowsAffected);

    public static DatabaseResult<T> Error(string errorMessage, ErrorTypes type, int rowsAffected = 0) => 
        new DatabaseResult<T>(
            default,
            DatabaseModel.Error(
                errorMessage, 
                (int)type
            ),
            rowsAffected  
        );
}
