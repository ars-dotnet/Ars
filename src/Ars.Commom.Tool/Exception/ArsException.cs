using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Ars.Common.Tool
{
    public class ArsException : Exception
    {
        public ArsException()
        {

        }

        public ArsException(string message) : base(message)
        {

        }

        public ArsException(string message, Exception innerException) : base(message,innerException)
        {

        }

        public ArsException(SerializationInfo info, StreamingContext context)
        {

        }
    }
}
