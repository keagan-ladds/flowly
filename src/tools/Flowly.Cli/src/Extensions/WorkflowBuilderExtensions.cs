using Flowly.Cli.Internal;
using Flowly.Core.Builders;
using Flowly.WorkflowSource.Extensions;

namespace Flowly.Cli.Extensions
{
    internal static class WorkflowBuilderExtensions
    {
        public static WorkflowBuilder FromRunnerOptions(this WorkflowBuilder builder, RunnerOptions opts) {
            builder.FromYamlFile(opts.Workflow);
            return builder;
        }
    }
}
