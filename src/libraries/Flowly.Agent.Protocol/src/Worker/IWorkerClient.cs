using Flowly.Core.Definitions;
using System.Threading.Tasks;

namespace Flowly.Agent.Protocol.Worker
{
    public interface IWorkerClient
    {
        Task RunAsync(WorkflowDefinition workflowDefinition);
    }
}
