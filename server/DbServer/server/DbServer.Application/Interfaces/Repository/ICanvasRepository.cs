using System;
using DbServer.Domain.Data.Entities;

namespace DbServer.Application.Interfaces.Repository;

public interface ICanvasRepository
{
    public CanvasEntity Get();
    public CanvasEntity Add();
    public CanvasEntity Delete();
    public CanvasEntity Update();
    public CanvasEntity[] GetMany();
    public CanvasEntity[] DeleteMany();
}
