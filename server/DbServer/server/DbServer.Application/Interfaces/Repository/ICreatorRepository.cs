using System;
using DbServer.Domain.Data.Entities;
using DbServer.Domain.Data.Results;

namespace DbServer.Application.Interfaces.Repository;

public interface ICreatorRepository
{
    public Task<DatabaseResult<CreationEntity>> Get(int id);
    public Task<DatabaseResult<CreationEntity>> Add(CreationEntity entity);
    public Task<DatabaseResult<CreationEntity>> Delete(int id);
}
