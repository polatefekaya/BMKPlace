using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestServer.Application.Interfaces;
using RestServer.Domain.Data.Entities;
using RestServer.Domain.Data.Settings;

namespace RestServer.Application.Services;

public class MessagePublishDictionaryService : IMessagePublishDictionaryService
{
    private readonly ILogger<MessagePublishDictionaryService> _logger;
    private readonly ClientSettings _userOptions;
    private readonly ClientSettings _canvasOptions;

    private Dictionary<Type, string> _exchanges;
    

    public MessagePublishDictionaryService(ILogger<MessagePublishDictionaryService> logger, IOptionsMonitor<ClientSettings> optionsMonitor){
        _logger = logger;
        
        _userOptions = optionsMonitor.Get(ClientSettings.User);
        _canvasOptions = optionsMonitor.Get(ClientSettings.Canvas);

        _exchanges = new Dictionary<Type, string>();
        AddToDictionary();
    }

    private void AddToDictionary(){
        _logger.LogInformation("Exchange names adding to Publish Dictionary depending on it's types");

        Add(typeof(UserEntity), _userOptions.ExchangeName);
        Add(typeof(IEnumerable<UserEntity>), _userOptions.ExchangeName);
        Add(typeof(CanvasEntity), _canvasOptions.ExchangeName);
        Add(typeof(IEnumerable<CanvasEntity>), _canvasOptions.ExchangeName);

    }

    public void Add(Type key, string value){
        _exchanges.Add(key, value);
        _logger.LogDebug("Exchange name added for {typeName} with {queueName}", key.Name, _exchanges[key]);
    }

    public string? Get(Type key){
        _logger.LogDebug("Retrieving value for {key} key", key.Name);
        return _exchanges[key];
    }
}
