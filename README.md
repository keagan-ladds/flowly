# Flowly

Flowly is a simplified and extensible framework for defining and running programmatic workflows. It is written in C# and consists of the `Flowly.Core` library, which is released as a NuGet package. This library offers core functionality for building and executing workflows. The framework is designed to be extensible, allowing workflow steps to be referenced from NuGet packages as extensions, which are loaded at runtime. Additionally, the `Flowly.Cli` tool enables users to run workflows from the terminal.

## Installation

To get started with Flowly, you can install the [Flowly.Core NuGet package](#link-to-core-nuget-package). Additional functionality can be added by installing one or more of the available extension libraries. Here are the core and extension packages:

- [Flowly.Core NuGet Package](#link-to-core-nuget-package): Install this package to access the core functionality of Flowly.
- [Flowly.Extensions.Json NuGet Package](#link-to-json-extension-nuget-package): Provides support for loading workflows from JSON files and streams.
- [Flowly.Extensions.Yaml NuGet Package](#link-to-yaml-extension-nuget-package): Provides support for loading workflows from YAML files and streams.
- [Flowly.Extensions.NLog NuGet Package](#link-to-nlog-extension-nuget-package): Provides support for logging to NLog.
- [Flowly.Extensions.NuGet NuGet Package](#link-to-nuget-extension-nuget-package): Provides support for loading extensions that contain Workflow Steps from a NuGet package.

### Example: Install the `Flowly.Core` NuGet Package

```bash
dotnet add package Flowly.Core
```



# Usage
## Define a Workflow Step
A workflow step is the smallest unit of execution for a workflow. All workflow steps must derive from the WorkflowStep class. Here's an example of creating a simple step named 'ExampleStep':

```csharp
public class ExampleStep : WorkflowStep
{
    public override ValueTask ExecuteAsync()
    {
        Logger.Info("We Are In ExampleStep");
        return new ValueTask();
    }
}
```

## Define a Workflow Step with Options
Workflow steps can have options that can be configured at load or runtime. Workflow steps requiring options must derive from the WorkflowStep<T> class. Options can be accessed within the step through the Options property. Here's an example of the same ExampleStep with options:

```csharp
public class ExampleStepOptions
{
    public int ValueA { get; set; }
    public string ValueB { get; set; }
}

public class ExampleStep : WorkflowStep<ExampleStepOptions>
{
    public override ValueTask ExecuteAsync()
    {
        Logger.Info("We Are In ExampleStep");
        Logger.Info("ValueA: {0}", Options.ValueA);
        Logger.Info("ValueB: {0}", Options.ValueB);
        return new ValueTask();
    }
}
```

## Define a Workflow Programmatically
Workflows can be defined programmatically using the WorkflowBuilder. Here's an example of creating a workflow with the 'ExampleStep' followed by the 'ExampleStepWithOptions':

```csharp
var workflow = new WorkflowBuilder()
    .AddStep<ExampleStep>()
    .AddStep<ExampleStepWithOptions, ExampleStepOptions>(opts =>
    {
        opts.ValueA = 1;
        opts.ValueB = "Test";
    })
    .Build();
```

# Contribution Guidelines
To contribute to Flowly, please follow the guidelines in CONTRIBUTING.md.

# License
This project is licensed under the MIT License.