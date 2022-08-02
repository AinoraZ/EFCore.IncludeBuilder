using System;

namespace Ainoraz.EFCore.IncludeBuilder.Exceptions;

/// <summary>
/// Represents errors converting from alternative syntax to 
/// regular Entity Framework Core Include/ThenInclude syntax.
/// </summary>
public class IncludeConversionFailedException : Exception
{
    /// <summary>
    /// Initializes new IncludeConversionFailedException.
    /// </summary>
    public IncludeConversionFailedException()
    {
    }

    /// <summary>
    /// Initializes new IncludeConversionFailedException with specified message.
    /// </summary>
    /// <param name="message">Message describing exception.</param>
    public IncludeConversionFailedException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes new IncludeConversionFailedException with specified message and inner exception.
    /// </summary>
    /// <param name="message">Message describing exception.</param>
    /// <param name="innerException">Related inner exception.</param>
    public IncludeConversionFailedException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
