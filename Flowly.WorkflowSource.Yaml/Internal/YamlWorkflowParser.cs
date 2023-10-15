using Flowly.Core.Definitions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using YamlDotNet;
using YamlDotNet.Serialization;

namespace Flowly.WorkflowSource.Yaml.Internal
{
    internal class YamlWorkflowParser
    {
        private readonly IDeserializer _deserializer;

        public YamlWorkflowParser()
        {
            _deserializer = new DeserializerBuilder().Build();
        }

        public WorkflowDefinition Parse(Stream stream)
        {
            using(TextReader reader = new StreamReader(stream)) 
            {
                return _deserializer.Deserialize<WorkflowDefinition>(reader);
            }
        }
    }
}
