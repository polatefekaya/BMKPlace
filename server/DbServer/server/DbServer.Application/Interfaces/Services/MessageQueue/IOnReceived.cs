using System;

namespace DbServer.Application.Interfaces.Services.MessageQueue;

public interface IOnReceived
{
    public void OnReceived();
}
