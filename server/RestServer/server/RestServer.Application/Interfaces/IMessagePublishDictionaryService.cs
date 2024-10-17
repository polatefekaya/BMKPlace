using System;

namespace RestServer.Application.Interfaces;

public interface IMessagePublishDictionaryService
{
    public void Add(Type key, string value);
    public string? Get(Type key);
}
