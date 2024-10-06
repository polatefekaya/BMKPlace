using System;
using DbServer.Domain.Data.Entities;

namespace DbServer.Application.Interfaces.Repository;

public interface IPixelRepository
{
    public PixelEntity Get();
    public PixelEntity Add();
    public PixelEntity Delete();
    public PixelEntity GetByPosition();
    public PixelEntity[] GetManyByDate();
    public PixelEntity[] GetManyByColor();
    public PixelEntity[] DeleteManyByDate();
    public PixelEntity[] DeleteManyByColor();
}
