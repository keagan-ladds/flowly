using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Dynamic;
using System.Text;
using Flowly.Core.Definitions;
using Scriban;
using Scriban.Parsing;
using Scriban.Runtime;

namespace Flowly.Core.Internal
{
    internal class TemplatePreprocessor
    {
        private static TemplatePreprocessor Instance = new TemplatePreprocessor();
        public static PreprocessAction TemplateProcessorAction = Instance.Test;
        public static WorkflowStepDefinitionPreProcessAction WorkflowStepDefinitionPreProcessAction = Instance.Test;

        private void Test(WorkflowContext context, WorkflowStep step)
        {
            Test(context, step.Variables);
        }

        void Test(WorkflowContext workflowContext, WorkflowStepDefinition definition)
        {
            if (definition.Options == null) return;

            var objects = new ScriptObject();
            objects.Import(workflowContext, renamer: member => member.Name);
            objects.Import(Environment.GetEnvironmentVariables(), renamer: member => member.Name);

            var context = new TemplateContext();

            context.PushGlobal(objects);

            IDictionary<string, object> options = definition.Options;

            Process(context, options);
        }

        object? Process(TemplateContext context, object? item)
        {
            if (item == null) return default;

            if (item is IDictionary<string, object>)
            {
                var variables = (IDictionary<string, object>)item;
                var keys = new List<string>(variables.Keys);
                foreach (var key in keys)
                {
                    variables[key] = Process(context, variables[key]);
                }
            }
            else if (item is IDictionary<object, object>)
            {
                var variables = (IDictionary<object, object>)item;
                var keys = new List<object>(variables.Keys);
                foreach (var key in keys)
                {
                    variables[key] = Process(context, variables[key]);
                }
            }
            else if (item is System.Collections.ICollection)
            {
                var collection = (System.Collections.ICollection)item;
                foreach (var nestedVariable in (System.Collections.ICollection)collection)
                {
                    Process(context, nestedVariable);
                }
            }
            else if (item is string expression)
            {
                var lexerOption = new LexerOptions() { Mode = ScriptMode.Default };
                var template = Template.Parse(expression, lexerOptions: lexerOption);
                return template.Evaluate(context);
            }

            return item;
        }

        

        void Test(WorkflowContext workflowContext,  WorkflowVariables variables)
        {
            var objects = new ScriptObject();
            objects.Import(workflowContext, renamer: member => member.Name);
            objects.Import(Environment.GetEnvironmentVariables(), renamer: member => member.Name);

            var context = new TemplateContext();

            context.PushGlobal(objects);

            foreach (var variable in variables)
            {
                if (variable.Value is string expression)
                {
                    var lexerOption = new LexerOptions() { Mode = ScriptMode.Default };
                    var template = Template.Parse(expression, lexerOptions: lexerOption);
                    var result =  template.Evaluate(context);
                    variables[variable.Key] = result;   
                }
            }
            
            
        }
    }
}
