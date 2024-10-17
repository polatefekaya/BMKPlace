using System;

namespace SRWorkerServer.Domain.Data.Settings;

public class ClientSettings
{
    public const string Pixel = "Pixel";
    public string ExchangeName { get; set; } = string.Empty;
    public string Tag {get; set;} = string.Empty;
    public string[] QueueNames {get; set;} = [];
}
