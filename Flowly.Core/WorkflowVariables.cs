using System.Collections.Generic;

namespace Flowly.Core
{
    public class WorkflowVariables
    {
        protected Dictionary<string, object> Variables = new Dictionary<string, object>();

        public WorkflowVariables(Dictionary<string, object>? variables = null)
        {
            if (variables != null)
                Variables = variables;
        }

        public T? GetValue<T>(string name) where T : class
        {
            if (Variables.ContainsKey(name))
                return (T)Variables[name];

            return default;
        }

        public string GetString(string name)
        {
            if (Variables.ContainsKey(name))
                return (string)Variables[name];

            return string.Empty;
        }

        public void SetValue(string name, object value)
        {
            if (string.IsNullOrEmpty(name))
                return;

            Variables[name] = value;
        }
    }
}
