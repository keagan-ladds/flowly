// See https://aka.ms/new-console-template for more information
using Flowly.Agent;
using Flowly.Agent.Protocol.Worker;
using Flowly.Extensions.NLog;
using Flowly.Extensions.NuGet;
using Flowly.Core.Logging;
using PipeMethodCalls;
using PipeMethodCalls.MessagePack;

NLogSource loggerSource = new NLogSource();
Logger.LoggerProvider = loggerSource.GetProvider();
var logger = Logger.GetLoggerInstance(nameof(Program));

logger.Info("Hello, World!");

var source = new NuGetExtensionSource
{
    PackageSources = new List<NuGet.Configuration.PackageSource>
    {
        new NuGet.Configuration.PackageSource(@"C:\Users\ladds\source\repos\laddstech\DigitalFoundry\src\libraries\src\bin\Debug")
    }
};

var worker = new AgentWorker(source, loggerSource);
var pipeClient = new PipeClientWithCallback<IWorkerServer, IWorkerClient>(new MessagePackPipeSerializer(), "mypipe", () => worker);
logger.Info("Trying to connect to agent");
await pipeClient.ConnectAsync();
logger.Info("Connected to agent");

await pipeClient.WaitForRemotePipeCloseAsync();