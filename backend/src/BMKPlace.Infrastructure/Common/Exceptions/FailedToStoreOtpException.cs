using System;
using SharedKernel.Exceptions;

namespace BMKPlace.Infrastructure.Common.Exceptions;

public class FailedToStoreOtpException : InfrastructureValidationException
{
    public FailedToStoreOtpException(string message) : base(message){}
    public FailedToStoreOtpException(string message, Exception exception) : base(message,exception){}
}
