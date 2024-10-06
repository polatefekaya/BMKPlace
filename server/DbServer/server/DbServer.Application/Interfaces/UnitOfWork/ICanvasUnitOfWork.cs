using System;
using DbServer.Domain.Data.Entities;

namespace DbServer.Application.Interfaces.UnitOfWork;

public interface ICanvasUnitOfWork
{
    public CanvasEntity Add();
    public CanvasEntity Delete();
    public UserEntity GetCreator();
    public CanvasEntity[] GetManyByUserId();
    public CanvasEntity[] DeleteManyByUserId();
    public UserEntity[] GetContributors();
}
