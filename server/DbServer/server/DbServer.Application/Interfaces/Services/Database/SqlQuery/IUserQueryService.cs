using System;
using DbServer.Domain.Data.Entities;

namespace DbServer.Application.Interfaces.Services.Database.SqlQuery;

public interface IUserQueryService : ISqlQueryService<UserEntity>
{
    public string UpdateById { get; }
}
