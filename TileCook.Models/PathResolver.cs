using System;
using System.IO;

namespace TileCook.Models
{
    public class PathResolver : IPathResolver
    {
        private readonly string _basePath;

        public PathResolver(string basePath)
        {
            this._basePath = basePath;
        }

        public string ResolvePath(string path)
        {
            return Path.IsPathRooted(path) ? path : Path.Combine(this._basePath, path);
        }
    }
}