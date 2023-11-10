using Flowly.Core.Definitions;
using Flowly.Core.Internal;
using Flowly.Core.Logging;
using Flowly.Core.Providers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Flowly.Core
{
    /// <summary>
    /// Represents a workflow runner responsible for executing a sequence of steps defined in a workflow.
    /// </summary>
    public sealed class WorkflowRunner : IWorkflowRunner
    {
        /// <summary>
        /// Gets or sets the extension source for loading workflow extensions.
        /// </summary>
        public IExtensionSource? ExtensionSource { get; set; }

        /// <summary>
        /// Gets or sets the workflow step factory used to create step instances.
        /// </summary>
        public IWorfklowStepFactory? WorfklowStepFactory { get; set; }

        /// <summary>
        /// Gets or sets the type resolver for resolving types needed in the workflow.
        /// </summary>
        public ITypeResolver? TypeResolver { get; set; }

        /// <summary>
        /// Gets or sets the runtime dependency resolver for handling dependencies during execution.
        /// </summary>
        public IRuntimeDependencyResolver? RuntimeDependencyResolver { get; set; }

        public ILoggerSource? LoggerSource { get; set; }

        /// <summary>
        /// Gets or sets the working directory for the workflow. Defaults to the current directory.
        /// </summary>
        public string WorkingDirectory { get; set; } = Directory.GetCurrentDirectory();

        private readonly ILogger _logger;

        public WorkflowRunner()
        {
            _logger = Logger.GetLoggerInstance(nameof(WorkflowRunner));
        }

        /// <summary>
        /// Asynchronously runs the specified workflow.
        /// </summary>
        /// <param name="workflow">The workflow definition to be executed.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task RunAsync(WorkflowDefinition workflow)
        {
            var context = new WorkflowContext(WorkingDirectory, workflow.Variables);

            var typeResolver = TypeResolver ?? new ReflectionTypeResolver();
            var stepFactory = WorfklowStepFactory ?? new WorfklowStepFactory();

            if (!CanResolveAllTypes(workflow, typeResolver) && ExtensionSource != null && workflow.Extensions.Any())
            {
                _logger.Debug("Can't resolve all types from the loaded assemblies and extensions have been specified.");
                if (RuntimeDependencyResolver != null)
                {
                    ExtensionSource.RuntimeDependencyResolver = RuntimeDependencyResolver;
                }

                var extensionProvider = ExtensionSource.BuildProvider();
                await extensionProvider.LoadAsync(workflow.Extensions.ToArray());

                // At this point we have extensions loaded, ensure that we can resolve types provided by the extensions.
                typeResolver = new ExtensionTypeResolver(extensionProvider);
            }

            if (!ValidateWorkflow(workflow, typeResolver))
                return;

            var loggerProvider = LoggerSource?.GetProvider();

            _logger.Debug("Workflow has been validated, moving on to instantiating all the steps");

            foreach (var step in workflow.Steps)
            {
                var stepInstance = stepFactory.CreateInstance(step, typeResolver);
                stepInstance.Variables = new WorkflowVariables(step.Variables);
                stepInstance.ContinueOnError = step.ContinueOnError;
                stepInstance.Logger = loggerProvider?.CreateLogger(step.Type) ?? Logger.GetLoggerInstance(step.Type);

                context.AddStep(stepInstance);
            }

            foreach (var step in context.Steps)
            {
                try
                {
                    await step.ExecuteInternalAsync();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "An error occurred while processing the workflow step {step}.", step.Name);
                    break;
                }
            }
        }

        private bool ValidateWorkflow(WorkflowDefinition workflow, ITypeResolver typeResolver)
        {
            var unresolvedTypes = GetUnresolvedTypes(workflow, typeResolver);

            if (unresolvedTypes.Any())
            {
                _logger.Error("The following workflow steps could not be resolved while preparing the workflow: {0}", 
                    string.Join(",", unresolvedTypes));

                return false;
            }

            return true;
        }

        private bool CanResolveAllTypes(WorkflowDefinition workflow, ITypeResolver typeResolver)
        {
            return !GetUnresolvedTypes(workflow, typeResolver).Any();
        }

        private List<string> GetUnresolvedTypes(WorkflowDefinition workflow, ITypeResolver typeResolver)
        {
            var unresolvedTypes = new List<string>();

            foreach (var step in workflow.Steps)
            {
                if (step.TypeHint == null && !typeResolver.TryResolveType(step.Type, out var type))
                {
                    unresolvedTypes.Add(step.Type);
                }
            }

            return unresolvedTypes;
        }
    }
}
