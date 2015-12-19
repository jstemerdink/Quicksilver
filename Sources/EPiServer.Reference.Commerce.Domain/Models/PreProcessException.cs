using System;
using System.Runtime.Serialization;

namespace EPiServer.Reference.Commerce.Domain.Models
{
    [Serializable]

    public class PreProcessException : Exception
    {
        public PreProcessException()
        {
        }

        public PreProcessException(string message) : base(message)
        {
            // Add any type-specific logic.
        }
        public PreProcessException(string message, Exception innerException) :
         base(message, innerException)
        {
            // Add any type-specific logic for inner exceptions.
        }
        protected PreProcessException(SerializationInfo info,
           StreamingContext context) : base(info, context)
        {
            // Implement type-specific serialization constructor logic.
        }

    }
}
