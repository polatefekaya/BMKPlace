using System;
using DbServer.Domain.Data.Entities;

namespace DbServer.Application.Interfaces.Repository;

public interface ICreatorRepository
{
    public Task<CreationEntity> Get();
    public Task<CreationEntity> Add();
    public Task<CreationEntity> Delete();
    public Task<CreationEntity[]> GetMany();
    public Task<CreationEntity[]> DeleteMany();
}
