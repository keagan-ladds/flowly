using Flowly.Core.Definitions;
using Flowly.Core.Providers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Flowly.Core.Builders
{
    public class WorkflowBuilder
    {
        private readonly List<Action<WorkflowDefinition>> _builderActions = new List<Action<WorkflowDefinition>>();
        private IWorkflowSource? _workflowSource;

        public ILoggerProvider? LoggerProvider { get; private set; }

        public WorkflowBuilder()
        {
            
        }

        public WorkflowBuilder SetVariable(string name, object value)
        {
            _builderActions.Add(workflow =>
            {
                if (!workflow.Variables.TryAdd(name, value))
                    workflow.Variables[name] = value;
            });
            
            return this;
        }

        public WorkflowBuilder AddStep<TStep>(string? name = default) where TStep : WorkflowStep
        {
            _builderActions.Add(workflow =>
            {
                var step = new WorkflowStepDefinition<TStep>(name);
                workflow.Steps.Add(step);
            });

            return this;
        }

        public WorkflowBuilder AddStep<TStep, TOptions>(TOptions options, string? name = default)
            where TStep : WorkflowStep<TOptions>
            where TOptions : class, new()
        {
            _builderActions.Add(workflow =>
            {
                var step = new WorkflowStepDefinition<TStep>(name);
                step.OptionsInstance = options;

                workflow.Steps.Add(step);
            });

            return this;
        }

        public WorkflowBuilder AddStep<TStep, TOptions>(Action<TOptions> action, string? name = default)
            where TStep : WorkflowStep<TOptions>
            where TOptions : class, new()
        {
            _builderActions.Add(workflow =>
            {
                var options = new TOptions();
                action(options);

                var step = new WorkflowStepDefinition<TStep>(name);
                step.OptionsInstance = options;

                workflow.Steps.Add(step);
            });

            return this;
        }

        public WorkflowBuilder WithSource(IWorkflowSource workflowSource)
        {
            _workflowSource = workflowSource;
            return this;
        }

        public async Task<WorkflowDefinition> BuildAsync()
        {
            var worklowDefinition = new WorkflowDefinition();

            if (_workflowSource != null)
            {
                var provider = _workflowSource.Build();

                await provider.LoadAsync();

                worklowDefinition = provider.Workflow;
            }

            foreach(var action in _builderActions)
            {
                action(worklowDefinition);
            }

            return worklowDefinition;
        }

        public WorkflowDefinition Build()
        {
            return BuildAsync()
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }

        
    }
}
