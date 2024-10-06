using System;
using DbServer.Domain.Data.Entities;

namespace DbServer.Application.Interfaces.UnitOfWork;

public interface IUserUnitOfWork
{
    public UserEntity GetByPixelId();
    public UserEntity DeleteByPixelId();
    public UserEntity Delete();
    public UserEntity[] DeleteManyByCanvasId();
    public UserEntity[] GetManyByCanvasId();
}
