using Flowly.Cli.Internal;
using Flowly.Cli.Options;
using Flowly.Core.Builders;
using Flowly.Extensions.NuGet;
using NuGet.Configuration;

namespace Flowly.Cli.Extensions
{
    internal static class RunnerBuilderExtensions
    {
        public static RunnerBuilder FromOptions(this RunnerBuilder builder, WorkflowRunCmdOptions opts)
        {
            var packageSources = new List<PackageSource>();


            foreach(var source in opts.PackageSources)
            {
                packageSources.Add(new PackageSource(source));
            }

            var nugetExtensionSource = new NuGetExtensionSource()
            {
                PackageSources = packageSources,
                BaseDirectory = opts.ApplicationDirectory,
            };
            var resolver = new RuntimeDependencyResolver();

            builder.WithExtensionSource(nugetExtensionSource);  
            builder.WithRuntimeDependencyResolver(resolver);

            if (!string.IsNullOrEmpty(opts.WorkingDirectory))
                builder.SetWorkingDirectory(opts.WorkingDirectory);


            return builder;
        }
    }
}
