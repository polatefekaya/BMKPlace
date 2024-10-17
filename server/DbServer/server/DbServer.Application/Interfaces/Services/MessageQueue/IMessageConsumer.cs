using System;

namespace DbServer.Application.Interfaces.Services.MessageQueue;

public interface IMessageConsumer
{
    void Consume(Func<int> operation);
    public IMessageConsumer Configure(string queueName);
}
