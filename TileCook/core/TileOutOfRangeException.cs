using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TileCook
{
    public class TileOutOfRangeException : Exception
    {
        public string ParamName;

        public TileOutOfRangeException(string message)
            : base(message) { }

        public TileOutOfRangeException(string message, string paramName)
            : base(message) 
        {
            this.ParamName = paramName;
        }

        protected TileOutOfRangeException(SerializationInfo info, StreamingContext ctxt)
            : base(info, ctxt) { }
    }
}
