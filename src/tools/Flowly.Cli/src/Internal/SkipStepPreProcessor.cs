using Flowly.Core;
using Flowly.Core.Conditions;

namespace Flowly.Cli.Internal
{
    internal class SkipStepPreProcessor : IPreProcessor
    {
        private readonly IEnumerable<string> _stepsToSkip;

        public SkipStepPreProcessor(IEnumerable<string> stepsToSkip)
        {
            _stepsToSkip = stepsToSkip;
        }

        public void Process(WorkflowStep workflowStep)
        {
            if (ShouldSkip(workflowStep))
            {
                //workflowStep.Condition = Condition.False;
            }
        }

        private bool ShouldSkip(WorkflowStep workflowStep)
        {
            return _stepsToSkip.Any(_ => string.Equals(_, workflowStep.Name));
        }
    }
}
