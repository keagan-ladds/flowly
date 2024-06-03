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

        public WorkflowRunCmdOptionsBinder(Option<DirectoryInfo> appDirOptions, Option<DirectoryInfo> workingDirOption, Option<string> workflowNameOption, Option<IEnumerable<string>> sourceOption, Option<FileInfo> workflowFileOption, Option<IEnumerable<string>> skipStepOption, Option<IEnumerable<string>> includeStepOption)
        {
            _appDirOptions = appDirOptions;
            _workingDirOption = workingDirOption;
            _workflowNameOption = workflowNameOption;
            _sourceOption = sourceOption;
            _workflowFileOption = workflowFileOption;
            _skipStepOption = skipStepOption;
            _includeStepOption = includeStepOption;
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
            };
        }
    }
}
