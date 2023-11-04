using Flowly.Cli.Options;
using Flowly.Cli.Providers;
using Flowly.Core.Builders;
using Flowly.Extensions.Yaml.Extensions;

namespace Flowly.Cli.Extensions
{
    internal static class WorkflowBuilderExtensions
    {
        public static WorkflowBuilder FromOptions(this WorkflowBuilder builder, WorkflowRunCmdOptions opts) {
            
            if (!string.IsNullOrEmpty(opts.WorkflowFile))
            {
                builder.FromYamlFile(opts.WorkflowFile);
            } 
            else if (!string.IsNullOrEmpty(opts.Workflow))
            {
                var sourceProvider = new WorkflowSourceProvider(opts.ApplicationDirectory);
                builder.WithSource(sourceProvider.GetSource(opts.Workflow));
            }
            
            return builder;
        }
    }
}
