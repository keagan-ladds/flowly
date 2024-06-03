using Flowly.Agent;
using Flowly.Agent.Protocol.Worker;
using Flowly.Core.Builders;
using Flowly.Extensions.Yaml;
using PipeMethodCalls;
using PipeMethodCalls.MessagePack;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddConsole();
// Add services to the container.

var app = builder.Build();
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Asp.Net Core logger");
logger.LogDebug("Debug");

// Configure the HTTP request pipeline.



var workflow = new WorkflowBuilder()
    .WithSource(new YamlFileWorkflowSource
    {
        Path = @"C:\Users\ladds\test\workflow-1.yaml"
    })
    .Build();

var pipeServer = new PipeServerWithCallback<IWorkerClient, IWorkerServer>(
    new MessagePackPipeSerializer(),
    "mypipe",
    () => new Agent());

logger.LogDebug("Ready to accept connections");
await pipeServer.WaitForConnectionAsync();

await pipeServer.InvokeAsync(_ => _.RunAsync(workflow));  

logger.LogDebug("New connection");

app.Run();
