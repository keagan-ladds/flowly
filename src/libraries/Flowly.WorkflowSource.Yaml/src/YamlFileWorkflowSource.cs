using Flowly.Core.Providers;

namespace Flowly.WorkflowSource.Yaml
{
    public class YamlFileWorkflowSource : IWorkflowSource
    {
        public string Path { get; set; }

        public IWorkflowProvider Build()
        {
            return new YamlFileWorkflowProvider
            {
                Path = Path
            };
        }
    }
}
