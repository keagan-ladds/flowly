using Flowly.Core.Providers;

namespace Flowly.Extensions.Yaml
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
