using System;

namespace DbServer.Domain.Data.Models.Errors;

public enum ErrorTypes
{
    RolledBack,
    NotFound,
    NotMade,
    CanNotMade,
    MutlipleInstances,
    Unexpected,
    NullResult,
}


