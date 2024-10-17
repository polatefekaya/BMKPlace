using System;
using DbServer.Application.Interfaces.Services.MessageQueue;
using DbServer.Domain.Data.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DbServer.Application.Services;

public class MessageService : IMessageService
{
    private readonly IMessageConsumerFactory _messageConsumerFactory;
    private readonly IOptions<MessageConsumerSettings> _consumerSettings;
    private readonly ILogger<MessageService> _logger;

    public MessageService(IMessageConsumerFactory messageConsumerFactory, IOptions<MessageConsumerSettings> consumerSettings, ILogger<MessageService> logger){
        _messageConsumerFactory = messageConsumerFactory;
        _consumerSettings = consumerSettings;
        _logger = logger;
    }
    public async Task Start()
    {
        foreach(string name in _consumerSettings.Value.QueueNames){
            _logger.LogInformation("{queueName} consumer creating", name);
            IMessageConsumer consumer = _messageConsumerFactory.Create(name);
            consumer.Consume(() => 1);
        }
    }

}
