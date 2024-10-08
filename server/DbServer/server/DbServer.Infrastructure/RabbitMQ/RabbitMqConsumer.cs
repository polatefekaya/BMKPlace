using System;
using DbServer.Application.Interfaces.Services.MessageQueue;
using DbServer.Infrastructure.RabbitMQ.Logic;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace DbServer.Infrastructure.RabbitMQ;

public class RabbitMqConsumer : IMessageConsumer
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly IOnReceived _onReceived;
    private readonly string _queueName;
    public RabbitMqConsumer(IConnectionFactory connectionFactory, IOnReceived onReceived, string queueName){
        _connectionFactory = connectionFactory;
        _onReceived = onReceived;
        _queueName = queueName;
    }

    public void Consume()
    {
        using IConnection connection = _connectionFactory.CreateConnection();
        using IModel channel = connection.CreateModel();

        declareQueue(channel);

        EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) => {
            _onReceived.OnReceived();
        }; 

        channel.BasicConsume(queue: _queueName, autoAck: true, consumer);
    }

    private void declareQueue(IModel channel){
        channel.QueueDeclare(queue: _queueName);
    }
}
