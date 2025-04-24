using System;
using Microsoft.Extensions.Logging;
using SharedKernel.Exceptions;
using SharedKernel.Extensions.String;

namespace BMKPlace.Application.Common.Helpers;

internal static class ValidationHelpers
{
    internal static void AddValidationError(
        Dictionary<string, List<string>> errors,
        string propertyName, 
        string errorMessage)
    {
        // Convert property name (e.g., "CanvasId") to camelCase ("canvasId") for API consistency
        string key = propertyName.ToCamelCase();

        if (!errors.TryGetValue(key, out List<string>? existingMessages))
        {
            existingMessages = new List<string>();
            errors.Add(key, existingMessages);
        }
        existingMessages.Add(errorMessage);
    }

    internal static void ThrowIfErrorsExist(
        Dictionary<string, List<string>> errors,
        string message,
        ILogger? logger = null,
        object? command = null)
    {
        if (errors.Count > 0)
        {
            var finalErrors = errors.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToArray());
            
            logger?.LogWarning("Input validation failed for command {CommandType}: {@ValidationErrors}",
                command?.GetType().Name ?? "Unknown",
                finalErrors);
            throw new ApplicationValidationException(message, finalErrors);
        }
    }
}
