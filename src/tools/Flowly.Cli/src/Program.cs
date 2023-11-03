using CommandLine;
using Flowly.Cli.Extensions;
using Flowly.Cli.Internal;
using Flowly.Core.Builders;
using Flowly.Core.Logging;
using Flowly.Extensions.NLog;
using NLog.Config;
using System.Reflection;

namespace Flowly.Cli
{
    internal class Program
    {
        static NLogSource loggerSource = new NLogSource();

        static int Main(string[] args)
        {
            var nlogConfigFile = GetEmbeddedResourceStream(Assembly.GetExecutingAssembly(), "NLog.config");
            if (nlogConfigFile != null)
            {
                var xmlReader = System.Xml.XmlReader.Create(nlogConfigFile);
                NLog.LogManager.Configuration = new XmlLoggingConfiguration(xmlReader, null);
            }

            Logger.LoggerProvider = loggerSource.GetProvider();

            return Parser.Default.ParseArguments<RunnerOptions>(args).MapResult(
               (RunnerOptions opts) => RunAndReturnExitCode(opts),
               errs => 1);
        }



        static int RunAndReturnExitCode(RunnerOptions opts)
        {
            var workflow = new WorkflowBuilder()
                 .FromRunnerOptions(opts)
                 .Build();

            var runner = new RunnerBuilder()
                .FromRunnerOptions(opts)
                .WithLoggerSource(loggerSource)
                .Build();

            runner.RunAsync(workflow).Wait();

            return 0;
        }

        public static Stream GetEmbeddedResourceStream(Assembly assembly, string resourceFileName)
        {
            var resourcePaths = assembly.GetManifestResourceNames()
              .Where(x => x.EndsWith(resourceFileName, StringComparison.OrdinalIgnoreCase))
              .ToList();
            if (resourcePaths.Count == 1)
            {
                return assembly.GetManifestResourceStream(resourcePaths.Single());
            }
            return null;
        }


    }
}