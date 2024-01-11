using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Ars.Common.Tool
{
    public class ArsException : System.Exception
    {
        public int Code { get; }

        public ArsException() : this(string.Empty)
        {
            
        }

        public ArsException(string message) : this(500, message) 
        {

        }


        public ArsException(int code,string message) : base(message)
        {
            Code = code;
        }

        public ArsException(string message, System.Exception innerException) : base(message,innerException)
        {
            Code = 500;
        }

        public ArsException(SerializationInfo info, StreamingContext context) : base(info,context)
        {
            Code = 500;
        }
    }
}
