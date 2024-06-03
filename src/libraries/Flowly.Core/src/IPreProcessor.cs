using System;
using System.Collections.Generic;
using System.Text;

namespace Flowly.Core
{
    public interface IPreProcessor
    {
        void Process(WorkflowStep workflowStep);
    }
}
