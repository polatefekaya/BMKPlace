using System;

namespace SharedKernel.Exceptions;

public class InfrastructureValidationException : Exception
{
    public InfrastructureValidationException(string message) : base(message) { }
    public InfrastructureValidationException(string message, Exception innerException) : base(message, innerException) { }
}
