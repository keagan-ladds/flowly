using Flowly.Core.Definitions;
using Flowly.Core.Interfaces;
using System;
using System.Threading.Tasks;

namespace Flowly.Core.Providers
{
    public abstract class WorkflowProvider : IWorkflowProvider, IDisposable
    {
        public WorkflowDefinition Workflow { get; protected set; }

        public abstract Task LoadAsync();
        public virtual void Dispose() { }
    }
}
