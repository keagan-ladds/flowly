using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Flowly.Core.Definitions
{
    /// <summary>
    /// Represents a definition for a workflow step in a workflow sequence.
    /// </summary>
    public class WorkflowStepDefinition
    {
        /// <summary>
        /// Gets or sets the type name of the workflow step.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the name of the workflow step.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets additional options associated with the workflow step.
        /// </summary>
        public ExpandoObject? Options { get; set; }

        /// <summary>
        /// Gets or sets a dictionary of variables associated with the workflow step.
        /// </summary>
        public Dictionary<string, object> Variables { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the workflow step should continue execution in case of an error.
        /// </summary>
        public bool ContinueOnError { get; set; } = false;

        /// <summary>
        /// Gets or sets the maximum number of retries on failure for the workflow step.
        /// </summary>
        public int RetryCountOnFailure { get; set; } = 0;

        internal virtual Type? TypeHint { get; set; }
        internal object? OptionsInstance { get; set; }
    }

    /// <summary>
    /// Represents a definition for a typed workflow step in a workflow sequence with additional options.
    /// </summary>
    /// <typeparam name="TJobStep">The type of the associated workflow step.</typeparam>
    public class WorkflowStepDefinition<TJobStep> : WorkflowStepDefinition
    {
        /// <summary>
        /// Gets the type hint for the workflow step definition, derived from the type parameter.
        /// </summary>
        internal override Type TypeHint => typeof(TJobStep);

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowStepDefinition{TJobStep}"/> class with the specified name.
        /// </summary>
        /// <param name="name">The name of the workflow step.</param>
        public WorkflowStepDefinition(string name)
        {
            Name = name;
            Type = TypeHint.Name;
        }
    }
}
