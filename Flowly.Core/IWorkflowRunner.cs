using Flowly.Core.Definitions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Flowly.Core
{
    public interface IWorkflowRunner
    {
        Task RunAsync(WorkflowDefinition workflow);
    }
}
