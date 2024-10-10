using System;
using DbServer.Application.Interfaces.Services.MessageQueue;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace DbServer.Infrastructure.RabbitMQ;

public class MessageConsumerFactory : IMessageConsumerFactory
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly ILogger<MessageConsumerFactory> _logger;
    private readonly ILogger<RabbitMqConsumer> _rmqLogger;
    public MessageConsumerFactory(IConnectionFactory connectionFactory, ILogger<MessageConsumerFactory> logger, ILogger<RabbitMqConsumer> rmqLogger){
        _connectionFactory = connectionFactory;
        _logger = logger;
        _rmqLogger = rmqLogger;
    }
    public IMessageConsumer Create(string queueName)
    {
        _logger.LogInformation("Creating new message consumer");
        return new RabbitMqConsumer(_connectionFactory, _rmqLogger, queueName);
    }
}
