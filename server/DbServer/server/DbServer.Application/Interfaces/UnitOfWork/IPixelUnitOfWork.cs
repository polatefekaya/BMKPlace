using System;
using DbServer.Domain.Data.Entities;

namespace DbServer.Application.Interfaces.UnitOfWork;

public interface IPixelUnitOfWork
{
    public PixelEntity[] GetManyByCanvasId();
    public PixelEntity[] GetManyByUserId();
    public PixelEntity[] DeleteManyByUserId();
    public PixelEntity[] DeleteManyByCanvasId();
    public PixelEntity Add();
    public PixelEntity[] DeleteManyByDate();
    public PixelEntity Delete();
    public PixelEntity[] DeleteManyByColor();
    
}
