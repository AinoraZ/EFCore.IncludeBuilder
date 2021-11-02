using System;
using System.Runtime.Serialization;

namespace EFCore.IncludeBuilder.Exceptions
{
    [Serializable]
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

        protected IncludeConversionFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
