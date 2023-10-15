using Flowly.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flowly.WorkflowSource.Yaml
{
    public class YamlFileWorkflowSource : IWorkflowSource
    {
        public string Path { get; set; }

        public IWorkflowProvider Build()
        {
            return new YamlFileWorkflowProvider
            {
                Path = Path
            };
        }
    }
}
