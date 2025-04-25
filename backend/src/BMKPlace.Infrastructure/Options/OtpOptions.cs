using System;
using System.ComponentModel.DataAnnotations;

namespace BMKPlace.Infrastructure.Options;

public class OtpOptions
{
    public const string SectionName = "Otp";

    [Range(4, 8)]
    public int Length {get; set;} = 6;

    [Range(1, 15)]
    public int ExpiryMinutes {get; set;} = 5;
}
