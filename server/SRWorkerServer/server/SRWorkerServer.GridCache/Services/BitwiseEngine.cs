using System;
using Microsoft.Extensions.Logging;
using SRWorkerServer.GridCache.Data.Options;
using SRWorkerServer.GridCache.Interfaces;

namespace SRWorkerServer.GridCache.Services;

public class BitwiseEngine : IBitwiseEngine
{
    private const byte MERGE_SLICE = 4;
    private const byte EXTRACT_PART = 4;
    private const byte SHIFT_AMOUNT = 4;

    private readonly ILogger<BitwiseEngine> _logger;
    private readonly IBitwiseService _bitwiseService;

    private readonly string _className;

    public BitwiseEngine(ILogger<BitwiseEngine> logger, IBitwiseService bitwiseService){
        _logger = logger;
        _bitwiseService = bitwiseService;

        _className = nameof(BitwiseEngine);
        
    }

    public async Task<byte> Shrink(byte first, byte second, BitwiseShrinkOptions? options = null)
    {
        _logger.LogDebug("{methodName} is starting in {className}", nameof(Shrink), _className);

        byte mergeSlice = options?.MergeSlice ?? MERGE_SLICE;
        byte extractPart = options?.ExtractPart ?? EXTRACT_PART;
        byte shiftAmount = options?.ShiftAmount ?? SHIFT_AMOUNT;

        _logger.LogTrace(
            "{className} is {isNull}, using {source} values",
            nameof(BitwiseShrinkOptions), 
            options is null ? "null" : "not null",
            options is null ? "hardcoded" : "options's"    
        );

        //Shrink Logic
        first = await _bitwiseService.Shift(first, shiftAmount);
        byte mergedValue = await _bitwiseService.Merge(first, second, mergeSlice);

        return mergedValue;
    }
}
