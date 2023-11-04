using System.CommandLine;
using System.CommandLine.Binding;

namespace Flowly.Cli.Options
{
    internal class WorkflowRunCmdOptions
    {

        internal static string DefaultApplicationDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".flowly");
        public string? Directory { get; set; }

        public string? WorkflowFile { get; set; }

        public string? Workflow { get; set; }

        public IEnumerable<string> PackageSources { get; set; } = new List<string>();

        public string ApplicationDirectory { get; set; } = DefaultApplicationDirectory;
    }

    internal class WorkflowRunCmdOptionsBinder : BinderBase<WorkflowRunCmdOptions>
    {
        private readonly Option<DirectoryInfo> _appDirOptions;
        private readonly Option<DirectoryInfo> _workingDirOption;
        private readonly Option<FileInfo> _workflowFileOption;
        private readonly Option<string> _workflowNameOption;
        private readonly Option<IEnumerable<string>> _sourceOption;

        public WorkflowRunCmdOptionsBinder(Option<DirectoryInfo> appDirOptions, Option<DirectoryInfo> workingDirOption, Option<string> workflowNameOption, Option<IEnumerable<string>> sourceOption, Option<FileInfo> workflowFileOption)
        {
            _appDirOptions = appDirOptions;
            _workingDirOption = workingDirOption;
            _workflowNameOption = workflowNameOption;
            _sourceOption = sourceOption;
            _workflowFileOption = workflowFileOption;
        }

        protected override WorkflowRunCmdOptions GetBoundValue(BindingContext bindingContext)
        {
            return new WorkflowRunCmdOptions
            {
                WorkflowFile = bindingContext.ParseResult.GetValueForOption(_workflowFileOption)?.FullName,
                Workflow = bindingContext.ParseResult.GetValueForOption(_workflowNameOption),
                PackageSources = bindingContext.ParseResult.GetValueForOption(_sourceOption) ?? Enumerable.Empty<string>(),
                ApplicationDirectory = bindingContext.ParseResult.GetValueForOption(_appDirOptions)?.FullName ?? WorkflowRunCmdOptions.DefaultApplicationDirectory,
                Directory = bindingContext.ParseResult.GetValueForOption(_workingDirOption)?.FullName
            };
        }
    }
}
