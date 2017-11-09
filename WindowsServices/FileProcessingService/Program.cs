using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Topshelf;

namespace ScanerProcessingService
{
    class Program
    {
        static readonly CancellationTokenSource ctsSource = new CancellationTokenSource();
        static readonly CancellationToken cancellationToken = ctsSource.Token;
        static void Main(string[] args)
        {
            var currentDir = AppDomain.CurrentDomain.BaseDirectory;
            var inDir = Path.Combine(currentDir, "in");
            var outDir = Path.Combine(currentDir, "out");
            var conf = new LoggingConfiguration();
            var fileTarget = new FileTarget()
            {
                Name = "Default",
                FileName = Path.Combine(currentDir, "log.txt"),
                Layout = "${date} ${message} ${onexception:inner=${exception:format=toString}}"
            };
            conf.AddTarget(fileTarget);
            conf.AddRuleForAllLevels(fileTarget);

            HostFactory.Run(
                hostConf => hostConf.Service<ImageService>(
                    s => {
                        s.ConstructUsing(() => new ImageService(inDir, outDir));
                        s.WhenStarted(serv => serv.Start(cancellationToken, ctsSource));
                        s.WhenStopped(serv => serv.Stop(ctsSource));
                    }
                ).UseNLog(new LogFactory(conf)));
        }
    }
}
