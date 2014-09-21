using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;

namespace TileCook
{
    
    public class DiskCache : ICache
    {
        private const int ACCESS_POLL_COUNT = 5;
        private const int ACCESS_POLL_INTERVAL = 100; //100 milliseconds

        private string _cacheDirectory;

        public DiskCache(string directory)
        {
            if (string.IsNullOrEmpty(directory))
            {
                throw new ArgumentNullException("DiskCache directory cannot be null");
            }
            this._cacheDirectory = directory;
        }

        public string CacheDirectory
        {
            get {return this._cacheDirectory;}
        }

        public byte[] Get(Coord coord, string format)
        {
            string path = this.buildPath(coord, format);
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

        public void Put(Coord coord, string format, byte[] image)
        {
            string path = this.buildPath(coord, format);
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

        public void Delete(Coord coord, string format)
        {
            string path = this.buildPath(coord, format);
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

        private string buildPath(Coord coord, string format)
        {
            string filename = coord.Y.ToString() + "." + format;
            string path = Path.Combine(
                this._cacheDirectory,
                coord.Z.ToString(),
                coord.Z.ToString(),
                filename
            );
            return path;
        }
    }
}
