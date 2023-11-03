using Flowly.Core.Providers;
using Flowly.Extensions.Yaml.Internal;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Flowly.Extensions.Yaml
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
