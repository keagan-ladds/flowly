using Flowly.Core;
using Flowly.Core.Conditions;

namespace Flowly.Cli.Internal
{
    internal class IncludeStepPreProcessor : IPreProcessor
    {
        private readonly IEnumerable<string> _stepsToInclude;

        public IncludeStepPreProcessor(IEnumerable<string> stepsToSkip)
        {
            _stepsToInclude = stepsToSkip;
        }

        public void Process(WorkflowStep workflowStep)
        {
            if (ShouldSkipStep(workflowStep))
            {
                //workflowStep.Condition = Condition.False;
            }
        }

        private bool ShouldSkipStep(WorkflowStep workflowStep)
        {
            return !_stepsToInclude.Any(_ => string.Equals(_, workflowStep.Name));
        }
    }
}
