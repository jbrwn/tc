using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TileCook
{
    public class InvalidTileFormatException : Exception
    {
        public InvalidTileFormatException(string message)
            : base(message) { }


        protected InvalidTileFormatException(SerializationInfo info, StreamingContext ctxt)
            : base(info, ctxt) { }
    }
}
