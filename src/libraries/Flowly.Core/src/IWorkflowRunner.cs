using Flowly.Core.Definitions;
using System.Threading.Tasks;

namespace Flowly.Core
{
    public interface IWorkflowRunner
    {
        Task RunAsync(WorkflowDefinition workflow);
    }
}
