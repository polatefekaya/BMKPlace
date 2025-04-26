using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace BMKPlace.Infrastructure.Identity;

public class ApplicationUser : IdentityUser<int>
{
    [Required]
    public int SchoolId {get; set;}
}
