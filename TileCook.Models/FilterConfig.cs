using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace TileCook.Models
{
    [DataContract]
    public class FilterConfig
    {
        public FilterConfig() { }

        [DataMember]
        public IEnumerable<EnvelopeConfig> Extents;
        [DataMember]
        public IEnumerable<string> Formats;
        [DataMember]
        public IEnumerable<Tuple<int, int>> ZoomRanges;
    }
}
