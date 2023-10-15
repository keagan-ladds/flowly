using Flowly.Core.Definitions;
using Flowly.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Flowly.Core.Internal
{
    public static class StepActivator
    {
        public static WorkflowStep CreateInstance(WorkflowStepDefinition workflowStepDefinition)
        {
            var jobStepType = GetJobStepType(workflowStepDefinition);

            if (IsInstanceOfGenericType(jobStepType, out var typedJobStep))
            {
                var optionsType = typedJobStep.GenericTypeArguments[0];

                var options = workflowStepDefinition.ObjectInstance;

                if (options == null)
                {
                    options = Activator.CreateInstance(optionsType);
                }
                
                if (workflowStepDefinition.Options != null)
                {
                    TypeHelper.Map(workflowStepDefinition.Options, options);
                }
                else if(!(options.GetType() == optionsType || options.GetType().IsSubclassOf(optionsType)))
                {
                    throw new StepActivatorException();
                }

                    

                var instance = Activator.CreateInstance(jobStepType);

                var methodInfo = typedJobStep.GetMethod("SetOptions", BindingFlags.Instance | BindingFlags.NonPublic);
                methodInfo.Invoke(instance, new object[] { options });
                return instance as WorkflowStep;

            }

            if (jobStepType.IsSubclassOf(typeof(WorkflowStep)))
            {
                var step = Activator.CreateInstance(jobStepType) as WorkflowStep;
                if (step != null)
                {
                    return step;
                }
            }

            

            throw new StepActivatorException();
        }

        static Type GetJobStepType(WorkflowStepDefinition jobStepDefinition)
        {
            if (jobStepDefinition.TypeHint != null)
                return jobStepDefinition.TypeHint;

            if (TryFindType(jobStepDefinition.Type, out var type))
                return type;

            throw new InvalidOperationException();
        }

        static Dictionary<string, Type> typeCache = new Dictionary<string, Type>();

        public static bool TryFindType(string typeName, out Type t)
        {
            lock (typeCache)
            {
                if (!typeCache.TryGetValue(typeName, out t))
                {
                    foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        t = a.GetType(typeName);
                        if (t != null)
                            break;
                    }
                    typeCache[typeName] = t; // perhaps null
                }
            }
            return t != null;
        }

        static bool IsInstanceOfGenericType(Type type, out Type typedJobStep)
        {
            typedJobStep = null;

            while (type != null)
            {
                if (type.IsGenericType &&
                    type.GetGenericTypeDefinition() == typeof(WorkflowStep<>))
                {
                    typedJobStep = type;
                    return true;
                }
                type = type.BaseType;
            }
            return false;
        }
    }
}
