using Flowly.Core.Definitions;
using Flowly.Core.Internal;
using Flowly.Core.Providers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Flowly.Core
{
    public sealed class WorkflowRunner : IWorkflowRunner
    {
        public IExtensionSource? ExtensionSource { get;  set; }
        public IWorfklowStepFactory? WorfklowStepFactory { get; set; }
        public ITypeResolver? TypeResolver { get; set; }
        public string WorkingDirectory { get; set; } = Directory.GetCurrentDirectory();

        public async Task RunAsync(WorkflowDefinition workflow)
        {
            var context = new WorkflowContext(WorkingDirectory, workflow.Variables);

            var typeResolver = TypeResolver ?? new ReflectionTypeResolver();
            var stepFactory = WorfklowStepFactory ?? new WorfklowStepFactory();

            if (!CanResolveAllTypes(workflow, typeResolver) && ExtensionSource != null && workflow.Extensions.Any())
            {
                var extensionProvider = ExtensionSource.BuildProvider();
                await extensionProvider.LoadAsync(workflow.Extensions.ToArray());

                // At this point we have extensions loaded, ensure that we can resolve types provided by the extensions.
                typeResolver = new ExtensionTypeResolver(extensionProvider);
            }

            if (!ValidateWorkflow(workflow, typeResolver))
                return;

            foreach(var step in workflow.Steps)
            {
                var stepInstance = stepFactory.CreateInstance(step, typeResolver);
                stepInstance.Variables = new WorkflowVariables(step.Variables);
                stepInstance.ContinueOnError = step.ContinueOnError;
                context.AddStep(stepInstance);
            }

            foreach(var step in context.Steps)
            {
                try
                {
                    await step.ExecuteAsync();
                    step.Successful = true;
                }
                catch(Exception)
                {
                    if (!step.ContinueOnError)
                        throw;
                }
                finally { 
                    step.Executed = true;
                }
            }
        }

        private bool ValidateWorkflow(WorkflowDefinition workflow, ITypeResolver typeResolver)
        {
            var unresolvedTypes = GetUnresolvedTypes(workflow, typeResolver);

            if (unresolvedTypes.Any())
            {
                Console.WriteLine("The following types could not be resolved:");
                foreach (var unresolvedType in unresolvedTypes)
                    Console.WriteLine(unresolvedType);

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

            foreach(var step in workflow.Steps)
            {
                if (step.TypeHint == null && !typeResolver.TryResolveType(step.Type, out var type)) {
                    unresolvedTypes.Add(step.Type);
                }
            }

            return unresolvedTypes;
        }
    }
}
