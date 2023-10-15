using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Flowly.Core.Definitions
{
    public class WorkflowStepDefinition
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public ExpandoObject? Options { get; set; }
        public bool ContinueOnError { get; set; } = false;
        virtual internal Type? TypeHint { get; set; }
        internal object? ObjectInstance { get; set; }
    }

    public class WorkflowStepDefinition<TJobStep> : WorkflowStepDefinition
    {
        internal override Type TypeHint => typeof(TJobStep);

        public WorkflowStepDefinition(string name)
        {
            Name = name;
            Type = TypeHint.Name;
        }
    }
}
