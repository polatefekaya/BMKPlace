using System;
using SharedKernel.Exceptions;

namespace BMKPlace.Infrastructure.Common.Exceptions;

public class JwtConfigurationMissingException : InfrastructureValidationException
{
    public JwtConfigurationMissingException(string message) : base(message){}
    public JwtConfigurationMissingException(string message, Exception exception) : base(message,exception){}
}
