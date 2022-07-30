using System;

namespace Ainoraz.EFCore.IncludeBuilder.Exceptions;

public class IncludeConversionFailedException : Exception
{
    public IncludeConversionFailedException()
    {
    }

    public IncludeConversionFailedException(string message) : base(message)
    {
    }

    public IncludeConversionFailedException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
