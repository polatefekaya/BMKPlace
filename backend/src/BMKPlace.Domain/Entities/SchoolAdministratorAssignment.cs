using System;

namespace BMKPlace.Domain.Entities;

public sealed class SchoolAdministratorAssignment
{
    public int UserId { get; private set; }
    public int SchoolId { get; private set; }
    public DateTimeOffset AssignedOnUtc { get; private set; }

    // Navigation properties can be added if needed for querying, but EF Core
    // can often work with just the foreign keys defined in configuration.
    // public ApplicationUser User { get; set; } // Requires reference to Infrastructure type - avoid!
    // public School School { get; set; }

    // Private constructor for EF Core
    private SchoolAdministratorAssignment() { }

    // Factory or public constructor (careful with direct instantiation)
    public static SchoolAdministratorAssignment Create(int userId, int schoolId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(userId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(schoolId);

        return new SchoolAdministratorAssignment
        {
            UserId = userId,
            SchoolId = schoolId,
            AssignedOnUtc = DateTimeOffset.UtcNow // Consider injecting IDateTimeService here if strict testability needed
        };
    }
}
