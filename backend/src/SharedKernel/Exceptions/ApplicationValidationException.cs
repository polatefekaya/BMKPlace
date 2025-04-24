using System;

namespace SharedKernel.Exceptions;

public class ApplicationValidationException : Exception
{
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    public ApplicationValidationException(string message, Dictionary<string, string[]> errors, Exception ex) : base(message, ex){
        Errors = errors;
    }
    public ApplicationValidationException(string message, Dictionary<string, string[]> errors) : base(message)
    {
        Errors = errors;
    }
     public ApplicationValidationException(string message) : base(message)
    {
        Errors = new Dictionary<string, string[]>();
    }
}
