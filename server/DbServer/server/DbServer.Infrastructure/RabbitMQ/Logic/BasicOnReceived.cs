using System;
using DbServer.Application.Interfaces.Services.MessageQueue;

namespace DbServer.Infrastructure.RabbitMQ.Logic;

public class BasicOnReceived : IOnReceived
{
    public void OnReceived(){
        Console.WriteLine("Message Received");
    }
}
