using Microsoft.Owin.Hosting;
using System;
using System.Net.Http;

using System;
using Microsoft.Owin;
using Owin;
using System.Web.Http;
using TileCook.API;
using TileCook.Models;
using Newtonsoft.Json;
using System.IO;

namespace TileCook.API.SelfHost
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = "http://localhost:8080/";

            // Start OWIN host 
            using (WebApp.Start<Startup>(url: url))
            {
                Console.WriteLine("Press [enter] to quit...");
                Console.ReadLine();
            }
        }
    }
}
