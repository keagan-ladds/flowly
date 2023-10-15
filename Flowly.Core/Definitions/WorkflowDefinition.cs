using System.Collections.Generic;

namespace Flowly.Core.Definitions
{
    public class WorkflowDefinition
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public List<WorkflowStepDefinition> Steps { get; set; } = new List<WorkflowStepDefinition>();
        public Dictionary<string, object> Variables { get; set; } = new Dictionary<string, object>();
        public List<ExtensionDefinition> Extensions { get; set; } = new List<ExtensionDefinition>();
        
        public string WorkingDirectory { get; set; }
    }
}
