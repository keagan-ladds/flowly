using Flowly.Core.Builders;
using Flowly.WorkflowSource.Yaml;

namespace Flowly.WorkflowSource.Extensions
{
    public static class WorkflowBuilderExtensions
    {
        public static WorkflowBuilder FromYamlFile(this WorkflowBuilder builder, string path)
        {
            return builder.WithSource(new YamlFileWorkflowSource
            {
                Path = path
            });
        }
    }
}
