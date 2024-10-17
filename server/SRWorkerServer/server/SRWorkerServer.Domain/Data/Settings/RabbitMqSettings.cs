using System;

namespace SRWorkerServer.Domain.Data.Settings;

public class RabbitMqSettings
{
    public string? HostName { get; set;}
    public string? UserName {get; set;}
    public string? Password {get; set;}
}
