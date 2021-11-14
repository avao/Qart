using System;
using Microsoft.Owin.Hosting;

namespace Qart.Wheels
{
    class MainClass
    {
        public static void Main (string[] args)
        {
            string baseUrl = "http://localhost:5000";
            using (WebApp.Start<Startup>(baseUrl))
            {
                Console.WriteLine("Press Enter to quit.");
                Console.ReadKey();
            }
        }
    }
}
