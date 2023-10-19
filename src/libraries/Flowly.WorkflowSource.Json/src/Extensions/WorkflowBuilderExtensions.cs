using Flowly.Core.Builders;
using Flowly.WorkflowSource.Json;

namespace Flowly.WorkflowSource.Extensions
{
    public static class WorkflowBuilderExtensions
    {
        public static WorkflowBuilder FromJsonFile(this WorkflowBuilder builder, string path)
        {
            return builder.WithSource(new JsonFileWorkflowSource
            {
                Path = path
            });
        }
    }
}
