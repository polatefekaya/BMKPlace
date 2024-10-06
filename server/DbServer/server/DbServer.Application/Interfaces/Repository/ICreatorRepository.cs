using System;
using DbServer.Domain.Data.Entities;

namespace DbServer.Application.Interfaces.Repository;

public interface ICreatorRepository
{
    public CreationEntity Get();
    public CreationEntity Add();
    public CreationEntity Delete();
    public CreationEntity[] GetMany();
    public CreationEntity[] DeleteMany();
}
