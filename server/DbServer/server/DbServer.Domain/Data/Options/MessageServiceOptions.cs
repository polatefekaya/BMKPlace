using System;

namespace DbServer.Domain.Data.Options;

public class MessageServiceOptions
{
    public string[] QueueNames {get; set;} = [];
    public bool RunParallel {get; set;} = false;
}
