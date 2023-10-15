using Flowly.Core.Providers;
using Flowly.WorkflowSource.Yaml.Internal;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Flowly.WorkflowSource.Yaml
{
    public class YamlFileWorkflowProvider : FileWorkflowProvider
    {
        protected override Task LoadAsync(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            try
            {
                Workflow = new YamlWorkflowParser().Parse(stream);
            }
            catch (Exception ex)
            {

            }

            return Task.CompletedTask;
        }

        public override void Dispose()
        {

        }
    }
}
