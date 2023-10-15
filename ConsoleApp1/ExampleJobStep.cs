using Flowly.Core;

namespace ConsoleApp1
{
    public class ExampleJobStep : WorkflowStep<ClassOptions>
    {
        public override ValueTask ExecuteAsync()
        {
            var test = Options.Y;
            var test2 = 0;
            return ValueTask.CompletedTask;
        }
    }

    public class ClassOptions
    {
        public int X { get; set; }
        public string Y { get; set; }
        public NestedOptions Child { get;set;}
    }

    public class NestedOptions
    {
        public string X { get; set; }
        public decimal Y { get; set;}
    }

    internal class OtherClass
    {

    }
}
