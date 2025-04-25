using System;
using System.ComponentModel.DataAnnotations;

namespace BMKPlace.Infrastructure.Options;

public class EmailOptions
{
    public const string SectionName = "Email";

    [Required]
    [EmailAddress]
    public string FromAddress { get; set; } = string.Empty;

    [Required]
    public string FromName { get; set; } = string.Empty;

    [Required]
    public string SmtpHost { get; set; } = string.Empty;

    [Range(1, 65535)]
    public int SmtpPort { get; set; } = 587;

    public string? SmtpUsername { get; set; }

    public string? SmtpPassword { get; set; }

    public bool UseStartTls { get; set; } = true;
}
