namespace Flowly.Core.Providers
{
    public interface IWorkflowSource
    {
        IWorkflowProvider Build();
    }
}
