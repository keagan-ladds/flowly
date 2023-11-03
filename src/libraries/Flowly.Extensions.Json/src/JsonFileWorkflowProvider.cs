using Flowly.Core.Providers;
using Flowly.WorkflowSource.Json.Internal;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Flowly.WorkflowSource.Json
{
    public class JsonFileWorkflowProvider : FileWorkflowProvider
    {
        protected override Task LoadAsync(Stream stream)
        {
            if (stream == null) 
                throw new ArgumentNullException("stream");  

            try
            {
                Workflow = new JsonWorkflowParser().Parse(stream);
            }
            catch(Exception ex)
            {

            }

            return Task.CompletedTask;
        }

        public override void Dispose()
        {

        }
    }
}
