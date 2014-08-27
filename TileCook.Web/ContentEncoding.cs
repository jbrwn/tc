using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TileCook.Web
{
    public static class ContentEncoding
    {
        public static string GetContentEncoding(byte[] bytes)
        {
            if (bytes[0] == 0x1f && bytes[1] == 0x8b)
            {
                return "gzip";
            }
            if (bytes[0] == 0x78 && bytes[1] == 0x9c)
            {
                return "deflate";
            }
            return string.Empty;
        }
    }
}