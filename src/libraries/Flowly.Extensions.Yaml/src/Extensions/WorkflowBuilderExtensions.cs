using Flowly.Core.Builders;

namespace Flowly.Extensions.Yaml.Extensions
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
