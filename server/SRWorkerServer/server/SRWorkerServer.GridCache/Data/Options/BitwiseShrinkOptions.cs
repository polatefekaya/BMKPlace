using System;

namespace SRWorkerServer.GridCache.Data.Options;

public class BitwiseShrinkOptions
{
    public byte MergeSlice {get; set;}
    public byte ExtractPart {get; set;}
    public byte ShiftAmount {get; set;}
}
