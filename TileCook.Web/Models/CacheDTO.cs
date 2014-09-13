﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;


namespace TileCook.Web.Models
{
    public class CacheDTO
    {
        public CacheDTO() { }

        [DataMember(IsRequired = true)]
        public string Type { get; set; }

        // DiskCache
        [DataMember]
        public string CacheDirectory { get; set; }

        // Mbtiles cache
        [DataMember]
        public string Database { get; set; }
        [DataMember]
        public string Format { get; set; }
    }
}