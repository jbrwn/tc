using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TileCook
{
    [DataContract]
    public class Grid
    {
        [DataMember(IsRequired = true)]
        public string name { get; set; }

        [DataMember(IsRequired = true)]
        public double scale { get; set; }

    }
}
