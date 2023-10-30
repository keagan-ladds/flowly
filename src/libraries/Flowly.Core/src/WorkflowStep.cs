using Flowly.Core.Exceptions;
using Flowly.Core.Logging;
using System.Threading.Tasks;

namespace Flowly.Core
{
    /// <summary>
    /// Represents an abstract base class for a workflow step in a workflow sequence.
    /// </summary>
    public abstract class WorkflowStep
    {
        /// <summary>
        /// Gets or sets the context for the workflow step's execution.
        /// </summary>
        public WorkflowContext Context { get; internal set; }

        /// <summary>
        /// Gets or sets the variables associated with the workflow step.
        /// </summary>
        public WorkflowVariables Variables { get; internal set; }

        /// <summary>
        /// Gets or sets a value indicating whether the workflow step should continue execution in case of an error.
        /// </summary>
        public bool ContinueOnError { get; internal set; }

        /// <summary>
        /// Gets or sets the maximum number of retries on failure for the workflow step.
        /// </summary>
        public int RetryCountOnFailure { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the execution status of the workflow step.
        /// </summary>
        public ExecutionStatus ExecutionStatus { get; internal set; } = ExecutionStatus.Pending;

        /// <summary>
        /// Gets or sets the logger for the workflow step.
        /// </summary>
        public ILogger Logger { get; internal set; } = new NullLogger();

        /// <summary>
        /// Sets the context for the workflow step's execution.
        /// </summary>
        /// <param name="context">The workflow context to be associated with the step.</param>
        internal void SetContext(WorkflowContext context)
        {
            Context = context;
        }

        /// <summary>
        /// Asynchronously executes the workflow step.
        /// </summary>
        /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
        public abstract ValueTask ExecuteAsync();

        /// <summary>
        /// Cancels the execution of the workflow step.
        /// </summary>
        /// <param name="skip">Indicates whether the subsequent steps should be skipped. If not specified, defaults to <c>null</c>.</param>
        protected void Cancel(bool? skip = null)
        {
            throw new StepExecutionException($"The step '{GetType().Name}' was cancelled.", true, skip);
        }

        /// <summary>
        /// Executes the workflow step internally, handling execution status and exceptions.
        /// </summary>
        internal async Task ExecuteInternalAsync()
        {
            try
            {
                ExecutionStatus = ExecutionStatus.Executing;

                await ExecuteAsync();

                ExecutionStatus = ExecutionStatus.Executed;
            }
            catch (StepExecutionException ex)
            {
                ExecutionStatus = ex.IsCancelled ? ExecutionStatus.Cancelled : ExecutionStatus.Failed;

                var continueOnError = ex.ContinueOnError ?? ContinueOnError;

                if (!continueOnError)
                    throw;
            }
            catch
            {
                if (!ContinueOnError)
                    throw;
            }
        }
    }

    /// <summary>
    /// Represents an abstract base class for a typed workflow step in a workflow sequence with additional options.
    /// </summary>
    /// <typeparam name="TOptions">The type of options associated with the step.</typeparam>
    public abstract class WorkflowStep<TOptions> : WorkflowStep where TOptions : class, new()
    {
        /// <summary>
        /// Gets or sets the options associated with the workflow step.
        /// </summary>
        protected TOptions Options { get; private set; } = new TOptions();

        /// <summary>
        /// Sets the options for the workflow step.
        /// </summary>
        /// <param name="options">The options to be associated with the step.</param>
        internal void SetOptions(TOptions options)
        {
            Options = options;
        }
    }

    /// <summary>
    /// Represents the execution status of a workflow step.
    /// </summary>
    public enum ExecutionStatus
    {
        /// <summary>
        /// The step is pending execution.
        /// </summary>
        Pending = 0,

        /// <summary>
        /// The step is currently executing.
        /// </summary>
        Executing,

        /// <summary>
        /// The step has been successfully executed.
        /// </summary>
        Executed,

        /// <summary>
        /// The step was cancelled during execution.
        /// </summary>
        Cancelled,

        /// <summary>
        /// The step execution failed.
        /// </summary>
        Failed
    }

}
