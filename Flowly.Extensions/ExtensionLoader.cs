using Flowly.Core.Definitions;
using Flowly.Extensions.Core.Internal;
using Microsoft.Extensions.DependencyModel;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.Protocol.Core.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;

namespace Flowly.Extensions.Core
{
    public class ExtensionLoader
    {
        public static PackageSource[] DefaultPackageSources = new[]
        {
            new PackageSource("https://api.nuget.org/v3/index.json"),
            new PackageSource(@"C:\Users\ladds\source\repos\Flowly\Flowly.Extension.Example\bin\Debug"),
        };

        public async Task LoadExtensions(ExtensionDefinition[] extensions, PackageSource[]? sources = default)
        {
            if (sources == null)
                sources  =DefaultPackageSources;


            // Define a source provider, with the main NuGet feed, plus my own feed.
            var sourceProvider = new PackageSourceProvider(NullSettings.Instance, sources);
                

            // Establish the source repository provider; the available providers come from our custom settings.
            var sourceRepositoryProvider = new SourceRepositoryProvider(sourceProvider, Repository.Provider.GetCoreV3());

            // Get the list of repositories.
            var repositories = sourceRepositoryProvider.GetRepositories();

            // Disposable source cache.
            using var sourceCacheContext = new SourceCacheContext();

            // You should use an actual logger here, this is a NuGet ILogger instance.
            var logger = new NullLogger();

           

            // Replace this with a proper cancellation token.
            var cancellationToken = CancellationToken.None;

            // The framework we're using.
            var targetFramework = NuGetFramework.ParseFolder("dotnet6.0");
            var allPackages = new HashSet<SourcePackageDependencyInfo>();

            var dependencyContext = DependencyContext.Default;

            foreach (var ext in extensions)
            {
                var packageIdentity = await NuGetPackageProvider.GetPackageIdentity(ext, sourceCacheContext, logger, repositories, cancellationToken);

                if (packageIdentity is null)
                {
                    throw new InvalidOperationException($"Cannot find package {ext.Package}.");
                }

                await NuGetPackageProvider.GetPackageDependencies(packageIdentity, sourceCacheContext, targetFramework, logger, repositories, dependencyContext, allPackages, cancellationToken);
            }

            var packagesToInstall = NuGetPackageProvider.GetPackagesToInstall(sourceRepositoryProvider, logger, extensions, allPackages);

            // Where do we want to install our packages?
            // For now we'll pop them in the .extensions folder.
            var packageDirectory = Path.Combine(Environment.CurrentDirectory, ".extensions");
            var nugetSettings = Settings.LoadDefaultSettings(packageDirectory);

            await NuGetPackageProvider.InstallPackages(sourceCacheContext, logger, packagesToInstall, packageDirectory, nugetSettings, cancellationToken);


            NuGetPackageProvider.LoadPackageAssemblies(packagesToInstall, packageDirectory, nugetSettings, targetFramework, cancellationToken);
            var targetFrameworkAttribute = Assembly.GetExecutingAssembly()
            .GetCustomAttributes(typeof(System.Runtime.Versioning.TargetFrameworkAttribute), false)
            .SingleOrDefault() as TargetFrameworkAttribute;

            int x = 0;

        }

        
    }
}
