using System;
using DbServer.Domain.Data.Entities;

namespace DbServer.Application.Interfaces.Repository;

public interface IUserRepository
{
    public UserEntity Get();
    public UserEntity Add();
    public UserEntity Delete();
    public UserEntity Update();
    public UserEntity[] GetMany();
    public UserEntity[] DeleteMany();
}
