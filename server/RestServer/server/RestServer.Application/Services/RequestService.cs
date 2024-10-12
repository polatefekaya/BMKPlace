using System;
using System.Threading.Channels;
using MessagePack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RestServer.Application.Interfaces;
using RestServer.Application.Interfaces.Infrastructure;
using RestServer.Domain.Data.Entities;

namespace RestServer.Application.Services;

public class RequestService : IRequestService
{
    private ILogger<RequestService> _logger;
    private readonly IMessageClient _messageClient;
    private readonly IChannelManagerService _chManager;
    private readonly IConfiguration _configuration;
    private readonly double _timeout;

    public RequestService(ILogger<RequestService> logger, IMessageClient messageClient, IChannelManagerService chManager, IConfiguration configuration){
        _logger = logger;
        _messageClient = messageClient;
        _chManager = chManager;
        _configuration = configuration;

        _timeout = Convert.ToDouble(configuration["ClientSettings:RequestTimeout"]);
    }
    public void Publish<T, TMessage>(TMessage entity, string queue)
    {
        _logger.LogInformation("Standard publish is started");
        _messageClient.SendMessage<T, TMessage>(entity, queue);
    }

    public async Task<T> PublishRPC<T,TMessage>(TMessage entity, string queue, bool channelPerCall)
    {
        Channel<byte[]>? ch = _chManager.Get(createNew: true);
        if (ch is null){
            _logger.LogError("Could not create internal channel for that request");
            throw new ArgumentNullException();
        }
        _messageClient.SendRPCMessage<T, TMessage>(entity, queue, ch, channelPerCall);

        using CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(_timeout));

        byte[] response = await ch.Reader.ReadAsync(cts.Token);

        T responseEntity = MessagePackSerializer.Deserialize<T>(response);
        return responseEntity;
    }
}
