using System;

namespace SRWorkerServer.Application.Interfaces.Infrastructure;

public interface IExchangeDictionaryService
{
    public void Add(Type key, string value);
    public string? Get(Type key);
}
