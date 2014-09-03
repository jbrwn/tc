using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Threading;

namespace TileCook
{
    [DataContract]
    public class DiskCache : ICache
    {
        private const int ACCESS_POLL_COUNT = 5;
        private const int ACCESS_POLL_INTERVAL = 100; //100 milliseconds

        private DiskCache() { }

        public DiskCache(string directory)
        {
            this.CacheDirectory = directory;
        }
        
        [DataMember(IsRequired=true)]
        public string CacheDirectory { get; set; }

        public byte[] Get(int z, int x, int y, string format)
        {
            string path = this.buildPath(z, x, y, format);
            byte[] img = null;

            int i = 0;
            while (true)
            {
                try
                {
                    img = File.ReadAllBytes(path);
                    break;
                }
                catch (FileNotFoundException e) { break; }
                catch (DirectoryNotFoundException e) { break; }
                catch (IOException e)
                {   
                    //is file being written? check for ERROR_SHARING_VIOLATION (HRESULT = -2147024864)
                    //.NET 4.5 required to access HResult directly
                    if (e.HResult == -2147024864 && i <= ACCESS_POLL_COUNT)
                    {
                        i++;
                        Thread.Sleep(ACCESS_POLL_INTERVAL);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return img;
        }

        public void Put(int z, int x, int y, string format, byte[] image)
        {
            string path = this.buildPath(z, x, y, format);
            int i = 0;
            while (true)
            {
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    File.WriteAllBytes(path, image);
                    break;
                }
                catch (IOException e)
                {
                    //is file being written? check for ERROR_SHARING_VIOLATION (HRESULT = -2147024864)
                    //.NET 4.5 required to access HResult directly
                    if (e.HResult == -2147024864 && i <= ACCESS_POLL_COUNT)
                    {
                        i++;
                        Thread.Sleep(ACCESS_POLL_INTERVAL);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        public void Delete(int z, int x, int y, string format)
        {
            string path = this.buildPath(z, x, y, format);
            int i = 0;
            while (true)
            {
                try
                {
                    File.Delete(path);
                    break;
                }
                catch (IOException e)
                {
                    //is file being written? check for ERROR_SHARING_VIOLATION (HRESULT = -2147024864)
                    //.NET 4.5 required to access HResult directly
                    if (e.HResult == -2147024864 && i <= ACCESS_POLL_COUNT)
                    {
                        i++;
                        Thread.Sleep(ACCESS_POLL_INTERVAL);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        private string buildPath(int z, int x, int y, string format)
        {
            string filename = y.ToString() + "." + format;
            string path = Path.Combine(
                this.CacheDirectory,
                z.ToString(),
                x.ToString(),
                filename
            );
            return path;
        }
    }
}
