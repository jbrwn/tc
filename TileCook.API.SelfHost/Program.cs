using Microsoft.Owin.Hosting;
using System;
using System.Net.Http;

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
