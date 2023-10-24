using System.Collections.Generic;

namespace Flowly.Core
{
    /// <summary>
    /// Represents a context for executing a workflow with a collection of workflow steps.
    /// </summary>
    public class WorkflowContext
    {
        private readonly List<WorkflowStep> _workflowSteps = new List<WorkflowStep>();

        /// <summary>
        /// Gets or sets the working directory for the workflow context.
        /// </summary>
        public string WorkingDirectory { get; internal set; }

        /// <summary>
        /// Gets or sets the variables associated with the workflow context.
        /// </summary>
        public WorkflowVariables Variables { get; internal set; }

        /// <summary>
        /// Gets an immutable list of workflow steps added to the context.
        /// </summary>
        public IReadOnlyList<WorkflowStep> Steps => _workflowSteps.AsReadOnly();

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowContext"/> class with the specified working directory and variables.
        /// </summary>
        /// <param name="workingDirectory">The working directory for the workflow context.</param>
        /// <param name="variables">An optional dictionary of variables associated with the workflow context.</param>
        public WorkflowContext(string workingDirectory, Dictionary<string, object>? variables = null)
        {
            WorkingDirectory = workingDirectory;
            Variables = new WorkflowVariables(variables);
        }

        /// <summary>
        /// Adds a workflow step to the context.
        /// </summary>
        /// <param name="step">The workflow step to be added to the context.</param>
        internal void AddStep(WorkflowStep step)
        {
            step.SetContext(this);
            _workflowSteps.Add(step);
        }
    }
}
