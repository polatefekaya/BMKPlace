using System;
using DbServer.Application.Interfaces.Services.MessageQueue;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace DbServer.Infrastructure.RabbitMQ;

public class MessageConsumerFactory : IMessageConsumerFactory
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly ILogger<MessageConsumerFactory> _logger;
    private readonly IMessageConsumer _messageConsumer;

    public MessageConsumerFactory(IConnectionFactory connectionFactory, ILogger<MessageConsumerFactory> logger, IMessageConsumer messageConsumer){
        _connectionFactory = connectionFactory;
        _logger = logger;
        _messageConsumer = messageConsumer;
    }
    public IMessageConsumer Create(string queueName)
    {
        _logger.LogInformation("Creating new message consumer");
        return _messageConsumer.Configure(queueName);
    }
}
