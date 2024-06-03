using System.Collections;
using System.Collections.Generic;

namespace Flowly.Core
{
    public class WorkflowVariables : IEnumerable<KeyValuePair<string, object>>
    {
        private Dictionary<string, object> _variables { get; } = new Dictionary<string, object>();

        public WorkflowVariables(Dictionary<string, object>? variables = null)
        {
            if (variables != null)
                _variables = variables;
        }

        public T? GetValue<T>(string name) where T : class
        {
            if (_variables.ContainsKey(name))
                return (T)_variables[name];

            return default;
        }

        public string GetString(string name)
        {
            if (_variables.ContainsKey(name))
                return (string)_variables[name];

            return string.Empty;
        }

        public void SetValue(string name, object value)
        {
            if (string.IsNullOrEmpty(name))
                return;

            _variables[name] = value;
        }

        public void SetValue<T>(string name, T value)
        {
            if (string.IsNullOrEmpty(name))
                return;

            _variables[name] = value;
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _variables.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _variables.GetEnumerator();
        }

        public object this[string key]
        {
            get { return _variables[key]; }
            set { _variables[key] = value;}
        }
    }
}
