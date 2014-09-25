using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace TileCook
{
    public class TileNotFoundException : Exception
    {
        public TileNotFoundException(string message)
            : base(message) { }

        public TileNotFoundException(string message, Exception innerException)
            : base(message, innerException) { }

        protected TileNotFoundException(SerializationInfo info, StreamingContext ctxt)
            : base(info, ctxt) { }
    }
}
