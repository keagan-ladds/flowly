using Flowly.Core.Definitions;
using System.Threading.Tasks;

namespace Flowly.Core
{
    /// <summary>
    /// Represents an interface for a workflow runner responsible for executing a sequence of steps defined in a workflow.
    /// </summary>
    public interface IWorkflowRunner
    {
        /// <summary>
        /// Asynchronously runs the specified workflow.
        /// </summary>
        /// <param name="workflow">The workflow definition to be executed.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task RunAsync(WorkflowDefinition workflow);
    }
}
