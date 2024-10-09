using System;
using DbServer.Domain.Data.Models.Errors;

namespace DbServer.Domain.Data.Results;

public class DatabaseResult<T>
{
    public T Data {get; set;}
    public DatabaseModel Model {get; set;}
}
