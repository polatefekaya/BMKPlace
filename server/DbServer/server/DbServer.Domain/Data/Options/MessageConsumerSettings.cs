using System;

namespace DbServer.Domain.Data.Options;

public class MessageConsumerSettings
{
    public bool IsParallel {get; set;}
    public string[] QueueNames {get; set;} = Array.Empty<string>();
}
