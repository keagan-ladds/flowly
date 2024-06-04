using Flowly.Core.Definitions;

namespace Flowly.Core
{
    public interface IPreProcessor
    {
        void Process(WorkflowStep workflowStep);
    }

    public interface IWorkflowStepDefinitionPreProcessor
    {
        void Process(WorkflowStepDefinition workflowStepDefinition);
    }
}
