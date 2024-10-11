using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RestServer.Application.Interfaces.Infrastructure;
using RestServer.Domain.Data.Settings;

namespace RestServer.Infrastructure.RabbitMQ;

public class RabbitMqClient : IMessageClient, IDisposable
{
    private readonly ILogger<RabbitMqClient> _logger;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ClientSettings _userOptions;
    private readonly ClientSettings _canvasOptions;
    private readonly ClientSettings _pixelOptions;

    public RabbitMqClient(ILogger<RabbitMqClient> logger, IConnection connection, IOptionsSnapshot<ClientSettings> options){
        _logger = logger;
        _connection = connection;

        _userOptions = options.Get(ClientSettings.User);
        _canvasOptions = options.Get(ClientSettings.Canvas);
        _pixelOptions = options.Get(ClientSettings.Pixel);

        _channel = _connection.CreateModel();

        DeclareQueues(ref _channel);
    }

    public void Dispose()
    {
        _channel?.Close();
    }

    public void SendMessage<T>(T message)
    {
        
    }

    private void DeclareQueues(ref IModel channel){
        channel.QueueDeclare(_userOptions.QueueName, true, false, false, null);
        channel.QueueDeclare(_canvasOptions.QueueName, true, false, false, null);
        channel.QueueDeclare(_pixelOptions.QueueName, true, false, false, null);
    }
}
