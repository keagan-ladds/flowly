using Flowly.Core.Definitions;

namespace Flowly.Core.Internal
{
    internal class WorfklowStepFactory : IWorfklowStepFactory
    {
        public WorkflowStep CreateInstance(WorkflowStepDefinition workflowStepDefinition, ITypeResolver typeResolver)
        {
            return StepActivator.CreateInstance(workflowStepDefinition, typeResolver);
        }
    }
}
