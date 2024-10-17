using System;
using System.Data;
using System.Data.SqlClient;
using DbServer.Application.Interfaces.UnitOfWork;
using DbServer.Domain.Data.Options;
using Microsoft.Extensions.Options;

namespace DbServer.Infrastructure;

public class UnitOfWork: IUnitOfWork
{
    private readonly string? _connectionString;
    private IDbConnection _connection;
    private IDbTransaction _transaction;
    private bool _disposed;

    public UnitOfWork(IOptions<DatabaseSettings> options)
    {
        _connectionString = options.Value.ConnectionString;
    }

    public IDbConnection Connection
    {
        get
        {
            if (_connection == null)
            {
                _connection = new SqlConnection(_connectionString);
                _connection.Open();
            }
            return _connection;
        }
    }

    public IDbTransaction Transaction
    {
        get { return _transaction; }
    }

    public void Begin()
    {
        if (_transaction != null)
        {
            throw new InvalidOperationException("Transaction already in progress");
        }
        _transaction = Connection.BeginTransaction();
    }

    public void Commit()
    {
        try
        {
            _transaction?.Commit();
        }
        catch
        {
            _transaction?.Rollback();
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                _transaction.Dispose();
                _transaction = null;
            }
        }
    }

    public void Rollback()
    {
        try
        {
            _transaction?.Rollback();
        }
        finally
        {
            if (_transaction != null)
            {
                _transaction.Dispose();
                _transaction = null;
            }
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _transaction?.Dispose();
                _connection?.Dispose();
            }
            _disposed = true;
        }
    }
}
