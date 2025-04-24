using System;

namespace SharedKernel.Extensions.String;

public static class StringExtensions
{
    public static string ToCamelCase(this string text){
        return text[0].ToString().ToLowerInvariant() + text[1..];
    }
}
