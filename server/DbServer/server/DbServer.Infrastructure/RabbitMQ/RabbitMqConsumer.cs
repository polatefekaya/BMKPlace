using System;
using DbServer.Application.Interfaces.Services.MessageQueue;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace DbServer.Infrastructure.RabbitMQ;

public class RabbitMqConsumer : IMessageConsumer
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly ILogger<RabbitMqConsumer> _logger;
    private string _queueName;
    public RabbitMqConsumer(IConnectionFactory connectionFactory, ILogger<RabbitMqConsumer> logger){
        _connectionFactory = connectionFactory;
        _logger = logger;
    }
    public IMessageConsumer Configure(string queueName){
        _queueName = queueName;
        return this;
    }
    public void Consume(Func<int> operation)
    {
        _logger.LogDebug("Connection creating");
        using IConnection connection = _connectionFactory.CreateConnection();

        _logger.LogDebug("Channel creating");
        using IModel channel = connection.CreateModel();

        _logger.LogDebug("Queue declaring");
        declareQueue(channel);

        EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) => {
            invoke(operation);
        }; 

        channel.BasicConsume(queue: _queueName, autoAck: true, consumer);
    }

    private void invoke(Func<int> operation)
    {
        operation.Invoke();
    }

    private void declareQueue(IModel channel){
        channel.QueueDeclare(queue: _queueName);
        _logger.LogDebug("{queueName} Queue declared", _queueName);
    }
}
