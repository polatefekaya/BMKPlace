using System;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestServer.Application.Interfaces;
using RestServer.Domain.Data.Settings;

namespace RestServer.Application.Services;

public class ChannelManagerService : IChannelManagerService
{
    private readonly ILogger<ChannelManagerService> _logger;
    private readonly ClientSettings _userOptions;
    private readonly ClientSettings _canvasOptions;

    private readonly Dictionary<string, Channel<byte[]>> _channels;

    public ChannelManagerService (ILogger<ChannelManagerService> logger, IOptionsMonitor<ClientSettings> options){
        _logger = logger;

        _userOptions = options.Get(ClientSettings.User);
        _canvasOptions = options.Get(ClientSettings.Canvas);

        _channels = new Dictionary<string, Channel<byte[]>>();

        InitializeNecessary();
    }

    private void InitializeNecessary(){ 
        _channels.Clear();
        InitializeChannels(_userOptions.QueueNames, _userOptions.Tag);
        InitializeChannels(_canvasOptions.QueueNames, _canvasOptions.Tag);
    }

    private void InitializeChannels(string[] queues, string tag){
        foreach(string queue in queues){
            string name = string.Join('-', tag, queue).ToLower();
            if(!_channels.ContainsKey(name)){
                _channels.Add(
                    name,
                    Channel.CreateUnbounded<byte[]>()
                );
            } 
        }
    }

    public Channel<byte[]>? Get(string key = "", bool createNew = false){
        if (createNew) return Channel.CreateUnbounded<byte[]>();

        if(!_channels.ContainsKey(key)){
            _logger.LogWarning("There is no Channel for this key: {key}", key);
            return null;
        }

        return _channels[key];
    }
}
