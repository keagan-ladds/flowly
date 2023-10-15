using System.Threading.Tasks;

namespace Flowly.Core
{
    public abstract class WorkflowStep
    {
        public WorkflowContext Context { get; private set; }

        internal void SetContext(WorkflowContext context)
        {
            Context = context;
        }

        public abstract ValueTask ExecuteAsync();
    }

    public abstract class WorkflowStep<TOptions> : WorkflowStep where TOptions : class, new()
    {
        protected TOptions Options { get; private set; } = new TOptions();

        internal void SetOptions(TOptions options)
        {
            Options = options;
        }
    }

}
