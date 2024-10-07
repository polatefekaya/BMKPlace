using System;

namespace DbServer.Application.Interfaces.Services.MessageQueue;

public interface IMessageConsumer
{
    Task Consume();
}
