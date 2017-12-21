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
using Castle.DynamicProxy;
using FileProcessingService;
using Topshelf;
using PostSharp.Patterns.Diagnostics;
using PostSharp.Patterns.Diagnostics.Backends.Console;
using PostSharp.Patterns.Diagnostics.Backends.NLog;

namespace ScanerProcessingService
{
    [Log(AttributeExclude = true)]
    class Program
    {
       
        static readonly CancellationTokenSource ctsSource = new CancellationTokenSource();
        static readonly CancellationToken cancellationToken = ctsSource.Token;
        private IImageService _imageService;
        static void Main(string[] args)
        {
            SetupNLogConfig();
            LoggingServices.DefaultBackend = new NLogLoggingBackend(); 

            var currentDir = AppDomain.CurrentDomain.BaseDirectory;
            var inDir = Path.Combine(currentDir, "in");
            var outDir = Path.Combine(currentDir, "out");

            var _imageService = LoggingProxy<IImageService>.Create(new ImageService(inDir, outDir)); // setup Dynamic Proxy for logging

            HostFactory.Run(
                hostConf => hostConf.Service<IImageService>(
                    s => {
                        s.ConstructUsing(() => _imageService);
                        s.WhenStarted(serv => serv.Start(cancellationToken, ctsSource));
                        s.WhenStopped(serv => serv.Stop(ctsSource));
                    }
                ).UseNLog(new LogFactory(SetupNLogConfig())));
        }

        private static LoggingConfiguration SetupNLogConfig()
        {
            var nlogConfig = new LoggingConfiguration();

            var fileTarget = new FileTarget("file")
            {
                Name = "Default",
                FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt"),
                Layout = "${date} ${message} ${onexception:inner=${exception:format=toString}}"
            };

            nlogConfig.AddTarget(fileTarget);
            nlogConfig.LoggingRules.Add(new LoggingRule("*", NLog.LogLevel.Debug, fileTarget));

           // var consoleTarget = new ConsoleTarget("console");
         //   nlogConfig.AddTarget(consoleTarget);
           // nlogConfig.LoggingRules.Add(new LoggingRule("*", NLog.LogLevel.Debug, consoleTarget));

            LogManager.Configuration = nlogConfig;
            LogManager.EnableLogging();

            return nlogConfig;
        }
    }
}
