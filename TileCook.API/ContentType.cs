using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TileCook.API
{
    public static class ContentType
    {
        public static string GetContentType(string extension)
        {
            string contentType = null;
            switch (extension.ToLower())
            {
                case "json":
                    contentType = "application/json";
                    break;
                case "pbf":
                    contentType = "application/x-protobuf";
                    break;
                case "png":
                    contentType = "image/png";
                    break;
                case "jpg":
                    contentType = "image/jpeg";
                    break;
            }
            return contentType;
        }
    }
}