﻿using Flowly.Core.Definitions;
using Microsoft.Extensions.DependencyModel;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.Protocol.Core.Types;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System;
using System.Threading.Tasks;
using Flowly.ExtensionSource.NuGet.Internal;
using System.Reflection;
using System.Linq;
using Flowly.Core;
using Flowly.Core.Providers;

namespace Flowly.ExtensionSource.NuGet
{
    public class NuGetExtensionProvider : IExtensionProvider
    {
        private readonly PackageSource[] _packageSources;

        public NuGetExtensionProvider(PackageSource[] packageSources)
        {
            _packageSources = packageSources;
        }

        private readonly Dictionary<string, Type> _availableExtensionTypes = new Dictionary<string, Type>();



        public Task LoadAsync(ExtensionDefinition[] extensions)
        {
            return LoadExtensions(extensions);
        }

        private async Task LoadExtensions(ExtensionDefinition[] extensions)
        {
            // Define a source provider, with the main NuGet feed, plus my own feed.
            var sourceProvider = new PackageSourceProvider(NullSettings.Instance, _packageSources);


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
                var packageIdentity = await NuGetExtensionResolver.GetPackageIdentity(ext, sourceCacheContext, logger, repositories, cancellationToken);

                if (packageIdentity is null)
                {
                    throw new InvalidOperationException($"Cannot find package {ext.Package}.");
                }

                await NuGetExtensionResolver.GetPackageDependencies(packageIdentity, sourceCacheContext, targetFramework, logger, repositories, dependencyContext, allPackages, cancellationToken);
            }

            var packagesToInstall = NuGetExtensionResolver.GetPackagesToInstall(sourceRepositoryProvider, logger, extensions, allPackages);

            // Where do we want to install our packages?
            // For now we'll pop them in the .extensions folder.
            var packageDirectory = Path.Combine(Environment.CurrentDirectory, ".extensions");
            var nugetSettings = Settings.LoadDefaultSettings(packageDirectory);

            var assemblies = await NuGetExtensionResolver.InstallPackages(sourceCacheContext, logger, packagesToInstall, packageDirectory, nugetSettings, cancellationToken);
            foreach(var assemblyPath in assemblies)
            {
                var assembly = Assembly.LoadFrom(assemblyPath);
                var exts = assembly.GetTypes().Where(_ => _.IsSubclassOf(typeof(WorkflowStep))).ToList();
                foreach(var ext in exts) {
                    _availableExtensionTypes.TryAdd(ext.FullName, ext);
                }

            }
        }

        public bool TryResolveType(string name, out Type type)
        {
            if (_availableExtensionTypes.TryGetValue(name, out type))
                return true;

            return false;
        }
    }
}
