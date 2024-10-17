using System;

namespace RestServer.Domain.Data.Settings;

public class ClientSettings
{
    public const string User = "User";
    public const string Canvas = "Canvas";
    public string ExchangeName { get; set; } = string.Empty;
    public string Tag {get; set;} = string.Empty;
    public string[] QueueNames {get; set;} = [];
}
