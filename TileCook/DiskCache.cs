using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization;

namespace TileCook
{
    [DataContract]
    public class DiskCache : ICache
    {

        private DiskCache() { }

        public DiskCache(string directory)
            : this(directory, 0) { }

        public DiskCache(string directory, int lifetime)
        {
            this.directory = directory;
            this.lifetime = lifetime;
        }
        
        [DataMember(IsRequired=true)]
        public string directory { get; set; }

        //lifetime
        [DataMember]
        public int lifetime { get; set; }

        public byte[] get(int z, int x, int y, string format)
        {
            string path = this.buildPath(z, x, y, format);
            byte[] img = null;
            try
            {
                img = File.ReadAllBytes(path);
            }
            catch (FileNotFoundException e) { }
            catch (DirectoryNotFoundException e) { }
            return img;
        }

        public void put(int z, int x, int y, string format, byte[] image)
        {
            string path = this.buildPath(z, x, y, format);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllBytes(path, image);
        }

        public void delete(int z, int x, int y, string format)
        {
            string path = this.buildPath(z, x, y, format);
            File.Delete(path);
        }

        private string buildPath(int z, int x, int y, string format)
        {
            string filename = y.ToString() + "." + format;
            string path = Path.Combine(
                this.directory,
                z.ToString(),
                x.ToString(),
                filename
            );
            return path;
        }
    }
}
