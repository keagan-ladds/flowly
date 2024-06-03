using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Dynamic;
using System.Text;
using Scriban;
using Scriban.Parsing;
using Scriban.Runtime;

namespace Flowly.Core.Internal
{
    internal class TemplatePreprocessor
    {
        private static TemplatePreprocessor Instance = new TemplatePreprocessor();
        public static PreprocessAction TemplateProcessorAction = Instance.Test;

        private void Test(WorkflowContext context, WorkflowStep step)
        {
            Test(context, step.Variables);
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
