using CommandLine;

namespace Flowly.Cli.Internal
{
    [Verb("run")]
    internal class RunnerOptions
    {
        [Option('d')]
        public string? Directory { get; set; }

        [Option('f', "file", Required = true)]
        public string Workflow { get; set; }

        [Option('s', "source")]
        public IEnumerable<string> PackageSources { get; set; } = new List<string>();
    }
}
