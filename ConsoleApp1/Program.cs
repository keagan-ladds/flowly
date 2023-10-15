// See https://aka.ms/new-console-template for more information


using Flowly.Core;
using Flowly.Core.Builders;
using Flowly.Core.Definitions;
using Flowly.ExtensionSource.NuGet;
using Flowly.WorkflowSource.Extensions;


var extensions = new[]
{
    new ExtensionDefinition
    {
        Package = "Flowly.Extension.Example"
    }
};

await new NuGetExtensionProvider().LoadExtensions(extensions);

var builder = new WorkflowBuilder();
var job = builder
    //.FromJsonFile(@"C:\Users\ladds\source\repos\Flowly\ConsoleApp1\workflow-1.json")
    .FromYamlFile(@"C:\Users\ladds\source\repos\Flowly\ConsoleApp1\workflow-1.yaml")
    .Build();


var runner = new WorkflowRunner(job);
await runner.RunAsync();
int x = 0;
