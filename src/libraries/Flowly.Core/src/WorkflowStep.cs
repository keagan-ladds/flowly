using Flowly.Core.Exceptions;
using Flowly.Core.Logging;
using System;
using System.Threading.Tasks;

namespace Flowly.Core
{
    public enum ExecutionStatus
    {
        Pending = 0,
        Executing,
        Executed,
        Cancelled,
        Failed
    }

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
        /// Gets or sets a value indicating whether the workflow step was executed successfully.
        /// </summary>
        public bool Successful { get; internal set; }

        public ExecutionStatus ExecutionStatus { get; internal set; } = ExecutionStatus.Pending;

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

        protected void Cancel(bool? skip = null)
        {
            throw new StepExecutionException($"The step '{GetType().Name}' was cancelled.", true, skip);
        }

        internal async Task ExecuteInternalAsync()
        {
            try
            {
                ExecutionStatus = ExecutionStatus.Executing;

                await ExecuteAsync();

                ExecutionStatus = ExecutionStatus.Executed;
            }
            catch(StepExecutionException ex)
            {
                if (ex.IsCancelled) 
                    ExecutionStatus = ExecutionStatus.Cancelled;
                else 
                    ExecutionStatus = ExecutionStatus.Failed;

                if (ex.ContinueOnError.HasValue)
                {
                    if (ex.ContinueOnError != true)
                        throw;
                } 
                else if (ContinueOnError == false)
                    throw;
            }
            catch(Exception ex)
            {
                if (ContinueOnError == false)
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

}
