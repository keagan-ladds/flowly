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
            var test3 = Context.Variables.GetValue<string>("Test");
            Console.WriteLine("Value of test3 is {0}", test3);

            return new ValueTask();
        }
    }
}
