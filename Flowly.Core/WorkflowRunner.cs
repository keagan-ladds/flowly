using Flowly.Core.Definitions;
using Flowly.Core.Exceptions;
using Flowly.Core.Internal;
using System;
using System.Threading.Tasks;

namespace Flowly.Core
{
    public class WorkflowRunner
    {
        private readonly WorkflowDefinition _workflow;

        public WorkflowRunner(WorkflowDefinition workflow)
        {
            _workflow = workflow;
        }

        public async Task RunAsync()
        {
            var context = new WorkflowContext();    

            foreach(var step in  _workflow.Steps)
            {
                try
                {
                    var stepInstance = StepActivator.CreateInstance(step);
                    stepInstance.SetContext(context);
                    await stepInstance.ExecuteAsync();
                }
                catch (StepActivatorException)
                {
                    throw;
                }
                catch(Exception)
                {
                    if (!step.ContinueOnError)
                        throw;
                }
            }
        }
    }
}
