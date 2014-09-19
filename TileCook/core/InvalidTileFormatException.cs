using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TileCook
{
    public class InvalidTileFormatException : ArgumentException
    {
        public InvalidTileFormatException(string message)
            : base(message) { }

        public InvalidTileFormatException(string message, string paramName)
            : base(message, paramName) { }

        protected InvalidTileFormatException(SerializationInfo info, StreamingContext ctxt)
            : base(info, ctxt) { }
    }
}
