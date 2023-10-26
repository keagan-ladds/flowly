using Flowly.Core.Providers;
using System;
using System.Collections.Generic;

namespace Flowly.Core.Builders
{
    public class RunnerBuilder
    {
        private readonly List<Action<WorkflowRunner>> _builderActions = new List<Action<WorkflowRunner>>();

        public RunnerBuilder WithExtensionSource(IExtensionSource extensionSource)
        {
            _builderActions.Add(runner => { runner.ExtensionSource = extensionSource; });
            return this;
        }

        public RunnerBuilder WithTypeResolver(ITypeResolver typeResolver)
        {
            _builderActions.Add(runner => { runner.TypeResolver = typeResolver; });
            return this;
        }

        public RunnerBuilder WithRuntimeDependencyResolver(IRuntimeDependencyResolver resolver)
        {
            _builderActions.Add(runner => { runner.RuntimeDependencyResolver = resolver; });
            return this;
        }

        public RunnerBuilder WithStepFactory(IWorfklowStepFactory worfklowStepFactory)
        {
            _builderActions.Add(runner => { runner.WorfklowStepFactory  = worfklowStepFactory; });
            return this;
        }

        public RunnerBuilder SetWorkingDirectory(string  workingDirectory)
        {
            _builderActions.Add(runner => { runner.WorkingDirectory = workingDirectory; });
            return this;
        }

        public RunnerBuilder WithLoggerSource(ILoggerSource loggerSource) 
        {
            _builderActions.Add(runner => { runner.LoggerSource = loggerSource; });
            return this;
        }

        public IWorkflowRunner Build()
        {
            var runner = new WorkflowRunner();

            foreach (var action in _builderActions)
            {
                action.Invoke(runner);
            }

            return runner;
        }
    }
}
