using Flowly.Core;
using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Flowly.Extension.Example
{
    public class ExampleStep : WorkflowStep
    {
        public override ValueTask ExecuteAsync()
        {
            Console.WriteLine("This is a my dynamically loaded step");

            return new ValueTask();
        }
    }
}
