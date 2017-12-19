using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using Serilog.Context;
using Serilog.Enrichers;

namespace Serilog.Sinks.Log4Net.Sample
{
    internal class Program
    {

        private const string OutputTemplate =
            "[SERILOG] {Timestamp:G} ({ThreadId}) {Level} {SourceContext} - {Message}{NewLine}{Exception}";

        private static void Main()
        {
            XmlConfigurator.Configure();

            Log.Logger = new LoggerConfiguration()
                .Enrich.With(new ThreadIdEnricher())
                .WriteTo.ColoredConsole(outputTemplate: OutputTemplate)
                .WriteTo.Log4Net(skipFrames: 3)
                .CreateLogger();

            var log4NetLogger = LogManager.GetLogger(typeof(Program));
            var serilogLogger = Log.ForContext<Program>();

            var username = Environment.UserName;

            log4NetLogger.InfoFormat("LOG4NET---Hello, running as {0}!", username);

            serilogLogger.Information("SERILOG---Hello, running as {Username}!", username);

            var p = new { firstname = "john", lastname = "doe" };
            serilogLogger.Information("SERILOG-custom property added for {user} {@p}", username, p);

            Console.ReadKey(true);
        }
    }
}
