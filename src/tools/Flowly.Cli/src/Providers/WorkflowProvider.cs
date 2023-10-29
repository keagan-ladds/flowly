using Flowly.Core.Definitions;
using Flowly.Core.Providers;

namespace Flowly.Cli.Providers
{
    internal class WorkflowProvider : IWorkflowProvider
    {
        public WorkflowDefinition Workflow { get; private set; }

        public Task LoadAsync()
        {
            throw new NotImplementedException();
        }
    }
}
