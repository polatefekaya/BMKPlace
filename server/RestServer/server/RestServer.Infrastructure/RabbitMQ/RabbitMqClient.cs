using System;
using System.Threading.Channels;
using MessagePack;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RestServer.Application.Interfaces;
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
    private readonly IMessagePublishDictionaryService _dictionaryService;
    private readonly StringComparer _comparer;

    public RabbitMqClient(ILogger<RabbitMqClient> logger, IConnection connection, IOptionsMonitor<ClientSettings> options, IMessagePublishDictionaryService dictionaryService){
        _logger = logger;
        _connection = connection;
        _dictionaryService = dictionaryService;

        _userOptions = options.Get(ClientSettings.User);
        _canvasOptions = options.Get(ClientSettings.Canvas);

        _channel = _connection.CreateModel();

        _comparer = StringComparer.Create(System.Globalization.CultureInfo.InvariantCulture, true);

        DeclareNecessary();
    }

    public void Dispose()
    {
        _channel?.Close();
    }

    public void SendMessage<T,TMessage>(TMessage message, string qName)
    {
        string? exchange = GetExchange(typeof(T));
        qName = FormatQueueName(qName);

        byte[] msg = MessagePackSerializer.Serialize(message);
        _logger.LogDebug("publishing to Exchange: {exchangeName}, Queue: {queueName}",exchange,  qName);

        IBasicProperties prop = _channel.CreateBasicProperties();
        prop.Persistent = true;

        _channel.BasicPublish(
            exchange: exchange, 
            routingKey: qName, 
            basicProperties: prop, 
            body: msg);
    }

    private void DeclareNecessary(){
        ExchangeWithQueues(_userOptions.ExchangeName, _userOptions.QueueNames, _userOptions.Tag);
        ExchangeWithQueues(_canvasOptions.ExchangeName, _canvasOptions.QueueNames, _canvasOptions.Tag);
    }


    private void ExchangeWithQueues(string exchange, string[] queues, string? tag = null){
        DeclareExchange(exchange);
        DeclareQueuesAndBind(queues, exchange, tag);
    }
    private void DeclareQueuesAndBind(string[] queues, string exchange, string? tag = null){
        foreach(string queue in queues){
            bool isRpc = _comparer.Compare(queue[..3],"get") == 0;

            string qName = tag is null ? queue : string.Join('-', tag, queue);

            if(isRpc){
                string resQName = FormatQueueName(qName, true, true);
                _channel.QueueDeclare(resQName, true, false, false, null);
                _channel.QueueBind(resQName, exchange, resQName, null);
            }

            qName = FormatQueueName(qName, isRpc);

            _channel.QueueDeclare(qName, true, false, false, null);
            _channel.QueueBind(qName, exchange, qName, null);
        }
    }

    private void DeclareExchange(string exchange){
        _channel.ExchangeDeclare(exchange, ExchangeType.Direct, true, false, null);
    }
    private string? GetExchange(Type objType){
        return _dictionaryService.Get(objType);
    }
    private string FormatQueueName(string qName, bool isRPC = false, bool isResponse = false){
        string newName = qName;

        if(isRPC){
            string rpcTag = isResponse ? "response" : "request";
            newName =  string.Join('-', qName, rpcTag);  
        }

        return newName.ToLower();
    }

    public void SendRPCMessage<T,TMessage>(TMessage message, string queueName, Channel<byte[]> channel, bool channelPerCall = false){
        string? exchange = GetExchange(typeof(T));
        string request = FormatQueueName(queueName, true);
        string response = FormatQueueName(queueName, true, true);

        string correlationId = Guid.NewGuid().ToString();

        _logger.LogDebug("\nRequest queue name: {requestQ}\nResponse queue name: {responseQ}\nCorrelation id: {corrId}", request, response, correlationId);

        IBasicProperties prop = _channel.CreateBasicProperties();
        prop.Persistent = true;
        prop.ReplyTo = response;
        prop.CorrelationId = correlationId;

        byte[] msg = MessagePackSerializer.Serialize(message);
        _logger.LogDebug("publishing RPC to Exchange: {exchangeName}, Queue: {queueName}",exchange,  request);

        _channel.BasicPublish(
            exchange: exchange,
            routingKey: request,
            basicProperties: prop,
            body: msg
        );

        StartRPCListening(channel, correlationId, response, channelPerCall);
    }

    private void StartRPCListening(Channel<byte[]> channel, string correlationId, string queue, bool channelPerCall = false){
        AsyncEventingBasicConsumer consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) => {
            bool isIdsSame = _comparer.Compare(ea.BasicProperties.CorrelationId, correlationId) == 0;
            if(isIdsSame){
                await channel.Writer.WriteAsync(ea.Body.ToArray());
                if(channelPerCall) channel.Writer.Complete();
            }
        };

        _channel.BasicConsume(
            queue: queue,
            autoAck: true,
            consumer: consumer
        );
    }
}
