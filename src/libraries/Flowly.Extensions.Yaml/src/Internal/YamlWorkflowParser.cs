using Flowly.Core.Definitions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using YamlDotNet;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Flowly.Extensions.Yaml.Internal
{
    internal class YamlWorkflowParser
    {
        private readonly IDeserializer _deserializer;

        public YamlWorkflowParser()
        {
            _deserializer = new DeserializerBuilder()
                .IgnoreUnmatchedProperties()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
        }

        public WorkflowDefinition Parse(Stream stream)
        {
            using (TextReader reader = new StreamReader(stream))
            {
                return _deserializer.Deserialize<WorkflowDefinition>(reader);
            }
        }
    }
}
