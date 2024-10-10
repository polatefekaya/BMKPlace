using System;
using DbServer.Application.Interfaces.Services.MessageQueue;
using DbServer.Domain.Data.Options;

namespace DbServer.Application.Services;

public class MessageService : IMessageService
{
    private readonly IMessageConsumerFactory _messageConsumerFactory;

    public MessageService(IMessageConsumerFactory messageConsumerFactory){
        _messageConsumerFactory = messageConsumerFactory;
    }
    public void Start(MessageServiceOptions? messageServiceOptions = null)
    {
        int count = 1;
        string[] names =["standard"];
        bool runParallel;
        List<IMessageConsumer> consumers = new List<IMessageConsumer>();

        if(messageServiceOptions is not null){
            count = messageServiceOptions.QueueNames.Count();
            names = messageServiceOptions.QueueNames;
            runParallel = messageServiceOptions.RunParallel;
        }

        for (int i = 0; i < count; i++)
        {
            consumers.Add(_messageConsumerFactory.Create(
                "sta"
            ));
        }
        for (int i = 0; i < count; i++)
        {
            consumers[i].Consume(() => 1);
        }
    }

}
