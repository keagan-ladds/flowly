// See https://aka.ms/new-console-template for more information


using Flowly.Core;
using Flowly.Core.Builders;
using Flowly.Core.Definitions;
using Flowly.ExtensionSource.NuGet;
using Flowly.WorkflowSource.Extensions;



var builder = new WorkflowBuilder();
var job = builder
    .FromYamlFile(@"workflow-1.yaml")
    //.SetVariable("Test", "This is really cool")
    .Build();


var extensionSource = new NuGetExtensionSource
{
    PackageSources = new List<NuGet.Configuration.PackageSource>
    {
        new NuGet.Configuration.PackageSource(@"C:\Users\ladds\source\repos\keagan-ladds\flowly\Flowly.Extension.Example\bin\Debug")
    }
};

var runner = new RunnerBuilder()
                .WithExtensionSource(extensionSource)
                .Build();

await runner.RunAsync(job);
int x = 0;
