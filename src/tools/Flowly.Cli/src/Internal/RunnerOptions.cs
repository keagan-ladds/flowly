using CommandLine;

namespace Flowly.Cli.Internal
{
    [Verb("run")]
    internal class RunnerOptions
    {
        [Option("working-directory")]
        public string? Directory { get; set; }

        [Option('f', "file", Required = true, SetName = "WorkflowSourceFile")]
        public string? WorkflowFile { get; set; }

        [Option('w', "workflow", Required = true, SetName = "WorkflowSourceName")]
        public string? Workflow { get; set; }

        [Option('s', "source")]
        public IEnumerable<string> PackageSources { get; set; } = new List<string>();
    }
}
