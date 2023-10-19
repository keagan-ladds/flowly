using Flowly.Core.Definitions;
using System;
using System.IO;
using System.Text.Json;

namespace Flowly.WorkflowSource.Json.Internal
{
    internal class JsonWorkflowParser
    {
        public WorkflowDefinition Parse(Stream stream)
        {
            var opts = new JsonSerializerOptions();
            opts.Converters.Add(new ExpandoObjectConverter());

            var workflow =  JsonSerializer.Deserialize<WorkflowDefinition>(stream, opts);

            if (workflow != null)
            {
                return workflow;
            }

            throw new InvalidOperationException();
        }
    }
}
