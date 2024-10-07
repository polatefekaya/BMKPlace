using System;
using System.Data;

namespace DbServer.Application.Interfaces.UnitOfWork;

public interface IUnitOfWork
{
    IDbConnection Connection { get; }
    IDbTransaction Transaction { get; }
    void Begin();
    void Commit();
    void Rollback();
}
