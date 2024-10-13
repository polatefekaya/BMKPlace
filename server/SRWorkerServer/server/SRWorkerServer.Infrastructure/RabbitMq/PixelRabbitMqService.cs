using System;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using SRWorkerServer.Application.Interfaces.Infrastructure;

namespace SRWorkerServer.Infrastructure.RabbitMq;

public class PixelRabbitMqService : IPixelMessageBrokerService
{
    private ILogger<PixelRabbitMqService> _logger;
    private readonly IMessageClient _rmqClient;

    public PixelRabbitMqService(ILogger<PixelRabbitMqService> logger, IMessageClient rmqClient){
        _logger = logger;
        _rmqClient = rmqClient;
    }
    public void PublishMessage<T, TMessage>(TMessage message, string queue)
    {
        _rmqClient.SendMessage<T, TMessage>(message, queue);
    }

    public void PublishRPCMessage<T, TMessage>(TMessage message, string queue, Channel<byte[]> channel, bool channelPerCall = false)
    {
        _rmqClient.SendRPCMessage<T, TMessage>(message, queue, channel, channelPerCall);
    }
}
