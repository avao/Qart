using System.Threading.Tasks;

namespace Qart.Wheels.CyberTester
{
    class Program
    {
        static Task<int> Main(string[] args)
        {
            return Qart.CyberTester.Program.ExecuteAsync(args, Startup.InstallServices, Startup.RegisterActions);
        }
    }
}
