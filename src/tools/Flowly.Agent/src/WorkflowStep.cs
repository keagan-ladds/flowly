using Flowly.Core;

namespace Flowly.Agent
{
    public class ExampleWorkflowStep : WorkflowStep
    {
        public override ValueTask ExecuteAsync()
        {
            Logger.Info($"In step {nameof(ExampleWorkflowStep)}");
            return ValueTask.CompletedTask;
        }
    }
}
