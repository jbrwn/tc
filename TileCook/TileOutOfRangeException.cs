using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TileCook
{
    class TileOutOfRangeException : Exception
    {
        public TileOutOfRangeException(string message)
            : base(message) { }


        protected TileOutOfRangeException(SerializationInfo info, StreamingContext ctxt)
            : base(info, ctxt) { }
    }
}
