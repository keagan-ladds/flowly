using Flowly.Core.Providers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flowly.WorkflowSource.Json
{
    public class JsonFileWorkflowSource : IWorkflowSource
    {
        public string Path { get; set; }

        public IWorkflowProvider Build()
        {
            return new JsonFileWorkflowProvider
            {
                Path = Path
            };
        }
    }
}
