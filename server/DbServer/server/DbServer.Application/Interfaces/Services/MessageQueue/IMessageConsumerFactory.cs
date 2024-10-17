using System;

namespace DbServer.Application.Interfaces.Services.MessageQueue;

public interface IMessageConsumerFactory
{
    public IMessageConsumer Create(string queueName);
}
