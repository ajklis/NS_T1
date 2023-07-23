using ConsoleApp1;
using Topshelf;
using uPLibrary.Networking.M2Mqtt;

namespace Listener;

class Program
{
    static void Main(string[] args)
    {
        // setting up as service
        var exitCode = HostFactory.Run(x =>
        {
            x.Service<ListenerService>(s =>
            {
                s.ConstructUsing(service => new ListenerService());
                s.WhenStarted(service => service.Start());
                s.WhenStopped(service => service.Stop());
            });

            x.RunAsLocalSystem();

            //basic info
            x.SetServiceName("ListenerService"); // machine friendly name
            x.SetDisplayName("Mqtt listener"); // user friendly name
            x.SetDescription("Mqtt listener changing data in local database"); // short description

        });

        int exitCodeValue = (int)Convert.ChangeType(exitCode, exitCode.GetTypeCode());
        Environment.ExitCode = exitCodeValue;

    }
}