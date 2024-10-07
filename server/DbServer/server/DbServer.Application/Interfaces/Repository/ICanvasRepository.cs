using System;
using DbServer.Domain.Data.Entities;

namespace DbServer.Application.Interfaces.Repository;

public interface ICanvasRepository
{
    public Task<CanvasEntity> Get();
    public Task<CanvasEntity> Add();
    public Task<CanvasEntity> Delete();
    public Task<CanvasEntity> Update();
    public Task<CanvasEntity[]> GetMany();
    public Task<CanvasEntity[]> DeleteMany();
}
