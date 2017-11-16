using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using NLog.Config;
using NLog.Targets;
using Topshelf;

namespace ConsoleApplication1
{
    class Program
    {
        static readonly CancellationTokenSource ctsSource = new CancellationTokenSource();
        static readonly CancellationToken cancellationToken = ctsSource.Token;
        static void Main(string[] args)
        {
            var currentDir = AppDomain.CurrentDomain.BaseDirectory;
            var outDir = Path.Combine(currentDir, "out");

            var serverConf = new LoggingConfiguration();
            var serverFileTarget = new FileTarget()
                                   {
                                       Name = "Default",
                                       FileName = Path.Combine(currentDir, "serverLog.txt"),
                                       Layout = "${date} ${message} ${onexception:inner=${exception:format=toString}}"
                                   };
            serverConf.AddTarget(serverFileTarget);
            serverConf.AddRuleForAllLevels(serverFileTarget);

            HostFactory.Run(
                hostConf => hostConf.Service<ServerSideService.ServerSideService>(
                    s => {
                        s.ConstructUsing(() => new ServerSideService.ServerSideService(outDir));
                        s.WhenStarted(serv => serv.Start(cancellationToken));
                        s.WhenStopped(serv => serv.Stop(ctsSource));
                    }
                ).UseNLog(new LogFactory(serverConf)));
        }
    }
}
