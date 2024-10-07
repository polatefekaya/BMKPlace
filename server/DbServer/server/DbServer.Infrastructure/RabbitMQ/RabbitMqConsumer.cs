using System;
using DbServer.Application.Interfaces.Services.MessageQueue;

namespace DbServer.Infrastructure.RabbitMQ;

public class RabbitMqConsumer : IMessageConsumer
{
    public Task Consume()
    {
        throw new NotImplementedException();
    }
}
