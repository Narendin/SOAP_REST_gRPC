using PumpClient.PumpServiceReference;
using System;
using System.ServiceModel;

namespace PumpClient
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var instanceContext = new InstanceContext(new CallBackHandler());
            var client = new PumpServiceClient(instanceContext);

            client.UpdateAndCompileScript(@"C:\Scripts\Sample.script");
            client.RunScript();

            Console.WriteLine("Please, Enter to exit...");
            Console.ReadKey(true);

            client.Close();
        }
    }
}