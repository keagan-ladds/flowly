using System.CommandLine;
using System.CommandLine.Binding;

namespace Flowly.Cli.Options
{
    internal class WorkflowRunCmdOptions
    {
        internal static string DefaultApplicationDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".flowly");
        public string? WorkingDirectory { get; set; }

        public string? WorkflowFile { get; set; }

        public string? Workflow { get; set; }

        public IEnumerable<string> PackageSources { get; set; } = new List<string>();

        public string ApplicationDirectory { get; set; } = DefaultApplicationDirectory;
        public IEnumerable<string> SkipStepNames { get; set; } = Enumerable.Empty<string>();
        public IEnumerable<string> IncludeStepNames { get; set; } = Enumerable.Empty<string>();
        public Dictionary<string, object> Variables { get; set; } = new Dictionary<string, object>();
    }

    internal class WorkflowRunCmdOptionsBinder : BinderBase<WorkflowRunCmdOptions>
    {
        private readonly Option<DirectoryInfo> _appDirOptions;
        private readonly Option<DirectoryInfo> _workingDirOption;
        private readonly Option<FileInfo> _workflowFileOption;
        private readonly Option<string> _workflowNameOption;
        private readonly Option<IEnumerable<string>> _sourceOption;
        private readonly Option<IEnumerable<string>> _skipStepOption;
        private readonly Option<IEnumerable<string>> _includeStepOption;
        private readonly Option<IEnumerable<string>> _setVariableOption;

        private static Dictionary<Type, Func<string, object>> VariableTypeParsers = new Dictionary<Type, Func<string, object>>()
        {
            { typeof(string), (value) => value },
            { typeof(int), (value) => int.Parse(value) },
        };

        public WorkflowRunCmdOptionsBinder(Option<DirectoryInfo> appDirOptions, Option<DirectoryInfo> workingDirOption, Option<string> workflowNameOption, Option<IEnumerable<string>> sourceOption, Option<FileInfo> workflowFileOption, Option<IEnumerable<string>> skipStepOption, Option<IEnumerable<string>> includeStepOption, Option<IEnumerable<string>> setVariableOption)
        {
            _appDirOptions = appDirOptions;
            _workingDirOption = workingDirOption;
            _workflowNameOption = workflowNameOption;
            _sourceOption = sourceOption;
            _workflowFileOption = workflowFileOption;
            _skipStepOption = skipStepOption;
            _includeStepOption = includeStepOption;
            _setVariableOption = setVariableOption;
        }

        protected override WorkflowRunCmdOptions GetBoundValue(BindingContext bindingContext)
        {
            return new WorkflowRunCmdOptions
            {
                WorkflowFile = bindingContext.ParseResult.GetValueForOption(_workflowFileOption)?.FullName,
                Workflow = bindingContext.ParseResult.GetValueForOption(_workflowNameOption),
                PackageSources = bindingContext.ParseResult.GetValueForOption(_sourceOption) ?? Enumerable.Empty<string>(),
                ApplicationDirectory = bindingContext.ParseResult.GetValueForOption(_appDirOptions)?.FullName ?? WorkflowRunCmdOptions.DefaultApplicationDirectory,
                WorkingDirectory = bindingContext.ParseResult.GetValueForOption(_workingDirOption)?.FullName,
                IncludeStepNames = bindingContext.ParseResult.GetValueForOption(_includeStepOption) ?? Enumerable.Empty<string>(),
                SkipStepNames = bindingContext.ParseResult.GetValueForOption(_skipStepOption) ?? Enumerable.Empty<string>(),
                Variables = ParseVariables(bindingContext.ParseResult.GetValueForOption(_setVariableOption) ?? Enumerable.Empty<string>())
            };
        }

        private Dictionary<string, object> ParseVariables(IEnumerable<string> setVariableOptions)
        {
            var variables = new Dictionary<string, object>();

            if (setVariableOptions == null || !setVariableOptions.Any())
                return variables;

            foreach(var setVariableOption in setVariableOptions)
            {
                if (TryParseVariable(setVariableOption, out KeyValuePair<string, object> variable))
                {
                    variables.Add(variable.Key, variable.Value);
                }
            }

            return variables;
        }

        private bool TryParseVariable(string setVariableOption, out KeyValuePair<string, object> variable)
        {
            variable = default;

            if (string.IsNullOrEmpty(setVariableOption)) return false;
            
            var variableParts = setVariableOption.Split('=');
            if (variableParts.Length != 2) return false;

            var valueType = typeof(string);
            var key = variableParts[0];

            if (variableParts[0].Contains(':'))
            {
                var keyParts = variableParts[0].Split(':');
                if (keyParts.Length != 2) return false;

                valueType = ParseValueType(keyParts[0]);
                key = keyParts[1];
            }

            if (TryParseVariableValue(valueType, variableParts[1], out object value))
            {
                variable = new KeyValuePair<string, object>(key, value);
                return true;
            }

            return false;
        }

        private bool TryParseVariableValue(Type valueType, string stringValue, out object value)
        {
            value = default;
            try
            {
                if (VariableTypeParsers.ContainsKey(valueType))
                {
                    value = VariableTypeParsers[valueType](stringValue);
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        private static Type ParseValueType(string type)
        {
            switch(type)
            {
                case "i":
                    return typeof(int);

                default:
                    return typeof(string);
            }
        }
    }
}
