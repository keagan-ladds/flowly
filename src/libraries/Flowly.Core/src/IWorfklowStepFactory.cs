using Flowly.Core.Definitions;

namespace Flowly.Core
{
    public interface IWorfklowStepFactory
    {
        WorkflowStep CreateInstance(WorkflowStepDefinition workflowDefinition, ITypeResolver typeResolver);
    }
}
