using CommandLine;
using Flowly.Cli.Extensions;
using Flowly.Cli.Internal;
using Flowly.Core.Builders;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Flowly.Cli
{
    internal class Program
    {
        static int Main(string[] args)
        {
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
                .Build();

            runner.RunAsync(workflow).Wait();

            return 0;
        }

        
    }
}