using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TileCook.Models
{
    public interface IPathResolver
    {
        string ResolvePath(string path);
    }
}