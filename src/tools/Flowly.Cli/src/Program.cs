using Flowly.Cli.Handlers;
using Flowly.Cli.Options;
using Flowly.Core.Logging;
using Flowly.Extensions.NLog;
using NLog.Config;
using System.CommandLine;
using System.Reflection;

namespace Flowly.Cli
{
    internal class Program
    {
        static NLogSource loggerSource = new NLogSource();

        static async Task<int> Main(string[] args)
        {
            var nlogConfigFile = GetEmbeddedResourceStream(Assembly.GetExecutingAssembly(), "NLog.config");
            if (nlogConfigFile != null)
            {
                var xmlReader = System.Xml.XmlReader.Create(nlogConfigFile);
                NLog.LogManager.Configuration = new XmlLoggingConfiguration(xmlReader, null);
            }

            Logger.LoggerProvider = loggerSource.GetProvider();

            
            return await BuildCommandLineParser().InvokeAsync(args);
        }

        static RootCommand BuildCommandLineParser()
        {
            var rootCommand = new RootCommand("Runs a workflow");
            var verboseOption = new Option<bool>("--verbose", "Enable verbose mode, which provides more detailed output for debugging and troubleshooting purposes.");
            var appDirOption = new Option<DirectoryInfo>("--appdir");

            var workflowFileOption = new Option<FileInfo>(new string[] { "-f", "--file" })
            {
                Arity = ArgumentArity.ZeroOrOne
            };

            var workflowNameOption = new Option<string>(new string[] { "-w", "--workflow" });
            var sourceOption = new Option<IEnumerable<string>>(new string[] { "-s", "--source" });
            var workingDirOption = new Option<DirectoryInfo>(new string[] { "-d", "--working-directory" });

            rootCommand.AddGlobalOption(verboseOption);
            rootCommand.AddGlobalOption(appDirOption);
            rootCommand.AddOption(workflowFileOption);
            rootCommand.AddOption(workflowNameOption);
            rootCommand.AddOption(sourceOption);
            rootCommand.AddOption(workingDirOption);
            rootCommand.AddValidator((context) =>
            {
                var workflowFile = context.GetValueForOption(workflowFileOption);
                var workflowName = context.GetValueForOption(workflowNameOption);

                if (workflowFile == null && string.IsNullOrEmpty(workflowName))
                    context.ErrorMessage = "Specifiy either the workflow file or workflow name.";
            });

            rootCommand.SetHandler(new WorkflowRunCmdHandler(loggerSource).HandleAsync,
                new WorkflowRunCmdOptionsBinder(appDirOption, workingDirOption, workflowNameOption, sourceOption, workflowFileOption));

            return rootCommand;
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