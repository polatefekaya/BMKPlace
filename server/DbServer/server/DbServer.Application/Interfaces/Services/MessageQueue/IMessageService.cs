using System;
using DbServer.Domain.Data.Options;

namespace DbServer.Application.Interfaces.Services.MessageQueue;

public interface IMessageService
{
    public void Start(MessageServiceOptions? messageServiceOptions = null);
}
