using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SRWorkerServer.Application.Interfaces.Infrastructure;
using SRWorkerServer.Domain.Data.Entities;
using SRWorkerServer.Domain.Data.Settings;

namespace SRWorkerServer.Infrastructure.RabbitMq;

public class ExchangeDictionaryService : IExchangeDictionaryService
{
    private readonly ILogger<ExchangeDictionaryService> _logger;
    private readonly ClientSettings _pixelOptions;

    private Dictionary<Type, string> _exchanges;
    

    public ExchangeDictionaryService(ILogger<ExchangeDictionaryService> logger, IOptionsMonitor<ClientSettings> optionsMonitor){
        _logger = logger;
        
        _pixelOptions = optionsMonitor.Get(ClientSettings.Pixel);

        _exchanges = new Dictionary<Type, string>();
        AddToDictionary();
    }

    private void AddToDictionary(){
        _logger.LogInformation("Exchange names adding to Publish Dictionary depending on it's types");

        Add(typeof(PixelEntity), _pixelOptions.ExchangeName);
        Add(typeof(IEnumerable<PixelEntity>), _pixelOptions.ExchangeName);

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
