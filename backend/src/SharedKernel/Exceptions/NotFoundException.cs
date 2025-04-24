using System;

namespace SharedKernel.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }

    public NotFoundException(string entityName, object key) : base($"{entityName} with key '{key}' was not found.") { }

    public NotFoundException(string message, Exception innerException) : base(message, innerException) { }
}
