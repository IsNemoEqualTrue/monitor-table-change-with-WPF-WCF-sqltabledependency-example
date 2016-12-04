using System;
using System.ServiceModel;
using ConsoleApplicationServer.PriceChangeService;

namespace ConsoleApplicationServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new ServiceHost(typeof(PriceTicker));
            host.Open();
            Console.WriteLine($"Service started at {host.Description.Endpoints[0].Address}");
            Console.WriteLine("Press key to stop the service.");
            Console.ReadLine();
            host.Close();
        }
    }
}