using Flowly.Cli.Internal;
using Flowly.Core.Builders;
using Flowly.ExtensionSource.NuGet;
using NuGet.Configuration;

namespace Flowly.Cli.Extensions
{
    internal static class RunnerBuilderExtensions
    {
        public static RunnerBuilder FromRunnerOptions(this RunnerBuilder builder, RunnerOptions opts)
        {
            var packageSources = new List<PackageSource>();

            foreach(var source in opts.PackageSources)
            {
                packageSources.Add(new PackageSource(source));
            }

            var baseDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".flowly");

            var nugetExtensionSource = new NuGetExtensionSource()
            {
                PackageSources = packageSources,
                BaseDirectory = baseDirectory,
            };
            var resolver = new RuntimeDependencyResolver();

            builder.WithExtensionSource(nugetExtensionSource);  
            builder.WithRuntimeDependencyResolver(resolver);

            if (!string.IsNullOrEmpty(opts.Directory))
                builder.SetWorkingDirectory(opts.Directory);

            return builder;
        }
    }
}
