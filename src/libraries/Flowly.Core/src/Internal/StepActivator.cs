using Flowly.Core.Definitions;
using Flowly.Core.Exceptions;
using System;
using System.Reflection;

namespace Flowly.Core.Internal
{
    public static class StepActivator
    {
        public static WorkflowStep CreateInstance(WorkflowStepDefinition workflowStepDefinition, ITypeResolver? typeResolver = null)
        {
            if (typeResolver == null)
                typeResolver = new ReflectionTypeResolver();

            var stepType = GetWorkflowStepType(workflowStepDefinition, typeResolver);

            if (stepType.IsInstanceOfGenericType(typeof(WorkflowStep<>), out var genericStep))
            {
                var optionsType = genericStep.GenericTypeArguments[0];

                var options = CreateOptionsInstance(workflowStepDefinition, optionsType);
                var stepInstance = Activator.CreateInstance(stepType);
               
                var methodInfo = genericStep.GetMethod("SetOptions", BindingFlags.Instance | BindingFlags.NonPublic);
                methodInfo.Invoke(stepInstance, new object[] { options });

                return stepInstance as WorkflowStep ?? throw new StepActivatorException();
            }

            if (stepType.IsSubclassOf(typeof(WorkflowStep)))
            {
                var stepInstance = Activator.CreateInstance(stepType) as WorkflowStep;
                return stepInstance ?? throw new StepActivatorException();
            }

            throw new StepActivatorException();
        }

        static object CreateOptionsInstance(WorkflowStepDefinition workflowStepDefinition, Type optionsType)
        {
            if (workflowStepDefinition.OptionsInstance != null)
                return workflowStepDefinition.OptionsInstance;

            var options = Activator.CreateInstance(optionsType);

            if (workflowStepDefinition.Options != null)
            {
                TypeMapper.Map(workflowStepDefinition.Options, options);
            }

            return options;
        }

        static Type GetWorkflowStepType(WorkflowStepDefinition jobStepDefinition, ITypeResolver typeResolver)
        {
            if (jobStepDefinition.TypeHint != null)
                return jobStepDefinition.TypeHint;

            if (typeResolver.TryResolveType(jobStepDefinition.Type, out var type))
                return type;

            throw new InvalidOperationException();
        }
    }
}
