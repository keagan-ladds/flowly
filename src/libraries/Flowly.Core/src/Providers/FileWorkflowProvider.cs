using System;
using System.IO;
using System.Threading.Tasks;

namespace Flowly.Core.Providers
{
    public abstract class FileWorkflowProvider : WorkflowProvider
    {
        public string Path { get; set; }

        public FileWorkflowProvider()
        {
            
        }

        public override Task LoadAsync()
        {
            if (string.IsNullOrEmpty(Path)) 
                throw new ArgumentNullException(nameof(Path)); 

            using (var fileStream = new FileStream(Path, FileMode.Open))
            {
                return LoadAsync(fileStream);
            }
        }

        protected abstract Task LoadAsync(Stream stream);

    }
}
