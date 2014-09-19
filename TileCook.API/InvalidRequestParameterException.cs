using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace TileCook.API
{
    public class InvalidRequestParameterException : Exception
    {
        public string ParamName;

        public InvalidRequestParameterException(string message)
            : base(message) { }

        public InvalidRequestParameterException(string message, string paramName) 
            : base(message) 
        {
            this.ParamName = paramName;
        } 

        protected InvalidRequestParameterException(SerializationInfo info, StreamingContext ctxt)
            : base(info, ctxt) { }
    }
}
