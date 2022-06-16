using System;
using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;
using Serilog.Enrichers;

namespace Serilog.Sinks.Log4Net.Sample
{
    internal class Program
    {

        private const string OutputTemplate =
            "[SERILOG] {Timestamp:G} ({ThreadId}) {Level} {SourceContext} - {Message}{NewLine}{Exception}";

        private static void Main()
        {
            XmlConfigurator.Configure(LogManager.GetRepository(Assembly.GetCallingAssembly()), new FileInfo("Serilog.Sinks.Log4Net.Sample.dll.config"));

            Log.Logger = new LoggerConfiguration()
                .Enrich.With(new ThreadIdEnricher())
                .WriteTo.Console(outputTemplate: OutputTemplate)
                .WriteTo.Log4Net()
                .CreateLogger();


            var log4NetLogger = LogManager.GetLogger(typeof (Program));
            var serilogLogger = Log.ForContext<Program>();

            var username = Environment.UserName;

            log4NetLogger.InfoFormat("Hello from log4net, running as {0}!", username);
            serilogLogger.Information("Hello from Serilog, running as {Username}!", username);

            Console.ReadKey(true);
        }
    }
}
