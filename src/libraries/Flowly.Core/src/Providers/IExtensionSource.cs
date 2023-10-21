namespace Flowly.Core.Providers
{
    public interface IExtensionSource
    {
        IRuntimeDependencyResolver? RuntimeDependencyResolver { get; set; }

        IExtensionProvider BuildProvider();
    }
}
