using System;
using DbServer.Application.Interfaces.Services.MessageQueue;

namespace DbServer.Web;

public class StartupTask : IHostedService
{
    private readonly IMessageService _messageService;
    public StartupTask(IMessageService messageService){
        _messageService = messageService;
    }
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _messageService.Start();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
