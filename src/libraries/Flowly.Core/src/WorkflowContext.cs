using System;
using System.Collections.Generic;
using System.Text;

namespace Flowly.Core
{
    public class WorkflowContext
    {
        private readonly List<WorkflowStep> _workflowSteps = new List<WorkflowStep>();

        public string WorkingDirectory { get; internal set; }
        public WorkflowVariables Variables { get; internal set; }
        public IReadOnlyList<WorkflowStep> Steps => _workflowSteps.AsReadOnly();
        
        public WorkflowContext(string workingDirectory, Dictionary<string, object>? variables = null)
        {
            WorkingDirectory = workingDirectory;
            Variables = new WorkflowVariables(variables);
        }

        public void AddStep(WorkflowStep step)
        {
            step.SetContext(this);
            _workflowSteps.Add(step);
        }
    }
}
