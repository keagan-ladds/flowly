using Flowly.Core.Providers;
using Flowly.WorkflowSource.Yaml;

namespace Flowly.Cli.Providers
{
    internal class WorkflowSourceProvider
    {
        private readonly string _applicationFilesDirectory;
        public WorkflowSourceProvider(string applicationFilesDirectory)
        {
            if (string.IsNullOrEmpty(applicationFilesDirectory)) 
                throw new ArgumentNullException(nameof(applicationFilesDirectory));

            _applicationFilesDirectory = applicationFilesDirectory;
        }

        public IWorkflowSource GetSource(string workflowName)
        {
            if (TryGetYamlWorkflowPathFromName(workflowName, out var workflowPath))
            {
                return new YamlFileWorkflowSource
                {
                    Path = workflowPath
                };
            }

            throw new InvalidOperationException($"Could not find the workflow file '{workflowName}'.");
        }

        private bool TryGetYamlWorkflowPathFromName(string workflowName, out string filePath)
        {
            var workflowDirectory = Path.Combine(_applicationFilesDirectory, "workflows");
            
            if (IsYamlFile(workflowName))
            {
                if (TryGetExistingFile(workflowDirectory, workflowName, out filePath))
                    return true;
            }

            if (TryGetExistingFile(workflowDirectory, $"{workflowName}.yml", out filePath))
                return true;

            if (TryGetExistingFile(workflowDirectory, $"{workflowName}.yaml", out filePath))
                return true;

            return false;
        }

        private static bool IsYamlFile(string fileName)
        {
            var extension = Path.GetExtension(fileName);

            if (string.IsNullOrEmpty(extension))
                return false;

            if (string.Equals(extension, ".yml", StringComparison.InvariantCultureIgnoreCase))
                return true;

            if (string.Equals(extension, ".yaml", StringComparison.InvariantCultureIgnoreCase))
                return true;

            return false;
        }

        private static bool TryGetExistingFile(string baseDirectory, string filename, out string filePath)
        {
            filePath = Path.Combine(baseDirectory, filename);
            return File.Exists(filePath);
        }
    }
}
