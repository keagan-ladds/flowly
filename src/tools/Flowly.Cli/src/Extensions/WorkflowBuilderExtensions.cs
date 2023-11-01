using Flowly.Cli.Internal;
using Flowly.Cli.Providers;
using Flowly.Core.Builders;
using Flowly.WorkflowSource.Extensions;

namespace Flowly.Cli.Extensions
{
    internal static class WorkflowBuilderExtensions
    {
        public static WorkflowBuilder FromRunnerOptions(this WorkflowBuilder builder, RunnerOptions opts) {
            
            if (!string.IsNullOrEmpty(opts.WorkflowFile))
            {
                builder.FromYamlFile(opts.WorkflowFile);
            } 
            else if (!string.IsNullOrEmpty(opts.Workflow))
            {
                var sourceProvider = new WorkflowSourceProvider(opts.ApplicationFilesDirectory);
                builder.WithSource(sourceProvider.GetSource(opts.Workflow));
            }
            
            return builder;
        }
    }
}
