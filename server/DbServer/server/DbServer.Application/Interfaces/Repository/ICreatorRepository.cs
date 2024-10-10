using System;
using DbServer.Domain.Data.Entities;
using DbServer.Domain.Data.Results;

namespace DbServer.Application.Interfaces.Repository;

public interface ICreatorRepository
{
    public Task<DatabaseResult<CreationEntity>> Get();
    public Task<DatabaseResult<CreationEntity>> Add();
    public Task<DatabaseResult<CreationEntity>> Delete();
    public Task<DatabaseResult<IEnumerable<CreationEntity>>> GetMany();
    public Task<DatabaseResult<IEnumerable<CreationEntity>>> DeleteMany();
}
