using System;
using DbServer.Application.Interfaces.Repository;
using DbServer.Application.Interfaces.UnitOfWork;
using DbServer.Domain.Data.Entities;

namespace DbServer.Infrastructure.Repository;

public class CanvasRepository : ICanvasRepository
{
    private readonly IUnitOfWork _unitOfWork;

    public CanvasRepository(IUnitOfWork unitOfWork){
        _unitOfWork = unitOfWork;
    }

    public Task<CanvasEntity> Add()
    {
        throw new NotImplementedException();
    }

    public Task<CanvasEntity> Delete()
    {
        throw new NotImplementedException();
    }

    public Task<CanvasEntity[]> DeleteMany()
    {
        throw new NotImplementedException();
    }

    public Task<CanvasEntity> Get()
    {
        throw new NotImplementedException();
    }

    public Task<CanvasEntity[]> GetMany()
    {
        throw new NotImplementedException();
    }

    public Task<CanvasEntity> Update()
    {
        throw new NotImplementedException();
    }
}
