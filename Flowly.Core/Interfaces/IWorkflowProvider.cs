using Flowly.Core.Definitions;
using System.Threading.Tasks;

namespace Flowly.Core.Interfaces
{
    public interface IWorkflowProvider
    {
        WorkflowDefinition Workflow { get; }
        Task LoadAsync();
    }
}
