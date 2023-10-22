using Flowly.Core.Definitions;
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
using NuGet.Versioning;
using NuGet.Packaging.Core;
using NuGet.Packaging;

namespace Flowly.ExtensionSource.NuGet
{
    public class NuGetExtensionProvider : IExtensionProvider
    {
        private readonly PackageSource[] _packageSources;
        private readonly Dictionary<string, Type> _availableExtensionTypes = new Dictionary<string, Type>();
        private readonly IRuntimeDependencyResolver? _runtimeDependencyResolver;
        private readonly List<Assembly> _assemblies = new List<Assembly>();
        private readonly List<RuntimeItem> _runtimeItems = new List<RuntimeItem>();

        public string BaseDirectory { get; private set; }

        public NuGetExtensionProvider(PackageSource[] packageSources, string baseDirectory, IRuntimeDependencyResolver? runtimeDependencyResolver = null)
        {
            _packageSources = packageSources;
            BaseDirectory = baseDirectory;
            _runtimeDependencyResolver = runtimeDependencyResolver;
        }

        public Task LoadAsync(ExtensionDefinition[] extensions)
        {
            return LoadExtensions(extensions);
        }

        private ISettings GetSettings()
        {
            string configFileName = "nuget.config";
            string configFilePath = Path.Combine(BaseDirectory, configFileName);

            if (File.Exists(configFilePath))
            {
                return Settings.LoadSpecificSettings(BaseDirectory, configFileName);
            }

            return NullSettings.Instance;
        }

        private async Task LoadExtensions(ExtensionDefinition[] extensions)
        {
            var settings = GetSettings();

            // Define a source provider, with the main NuGet feed, plus my own feed.
            var sourceProvider = new PackageSourceProvider(settings, _packageSources);

            // Establish the source repository provider; the available providers come from our custom settings.
            var sourceRepositoryProvider = new SourceRepositoryProvider(sourceProvider, Repository.Provider.GetCoreV3());

            // Get the list of repositories.
            var repositories = sourceRepositoryProvider.GetRepositories();

            var dummyPackage = new SourcePackageDependencyInfo("Flowly.Core", new NuGetVersion("0.0.0"), new List<PackageDependency>(), false, repositories.First());

            // Disposable source cache.
            using var sourceCacheContext = new SourceCacheContext();

            // You should use an actual logger here, this is a NuGet ILogger instance.
            var logger = new NullLogger();

            // Replace this with a proper cancellation token.
            var cancellationToken = CancellationToken.None;

            // The framework we're using.
            var targetFramework = NuGetFramework.ParseFolder("netstandard2.1");
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

            allPackages.Add(dummyPackage);
            var packagesToInstall = NuGetExtensionResolver.GetPackagesToInstall(sourceRepositoryProvider, logger, extensions, allPackages);

            // Where do we want to install our packages?
            // For now we'll pop them in the .extensions folder.
            var packageDirectory = Path.Combine(BaseDirectory, "extensions");
            var nugetSettings = Settings.LoadDefaultSettings(packageDirectory);

            try
            {
                var downloadResults = await NuGetExtensionResolver.InstallPackages(sourceCacheContext, logger, packagesToInstall, packageDirectory, nugetSettings, cancellationToken);
                var frameworkReducer = new FrameworkReducer();
                var framework = NuGetFramework.Parse("net6.0");

                foreach (var result in downloadResults)
                {
                    var package = result.PackageReader.GetIdentity();
                    var baseDir = Path.Combine(packageDirectory, $"{package.Id}.{package.Version}");

                    var libItems = result.PackageReader.GetLibItems();
                    var nearest = frameworkReducer.GetNearest(framework, libItems.Select(x => x.TargetFramework));
                    var extensionAssemblies = libItems
                        .Where(x => x.TargetFramework.Equals(nearest))
                        .SelectMany(x => x.Items)
                        .Where(_ => _.EndsWith(".dll"))
                        .Select(x => Path.Combine(baseDir, x));

                    LoadLibraryItems(extensionAssemblies);

                    var runtimeItems = result.PackageReader.GetItems("runtimes");
                    LoadRuntimeItems(runtimeItems, baseDir);

                }

            }
            catch (Exception ex)
            {
                int x = 0;
            }
        }

        private void LoadLibraryItems(IEnumerable<string> assemblies)
        {
            foreach (var assemblyPath in assemblies)
            {
                try
                {
                    var assembly = Assembly.LoadFrom(assemblyPath);
                    _assemblies.Add(assembly);
                    _runtimeDependencyResolver?.ResolveForAssembly(this, assembly);

                    var exts = assembly.GetTypes().Where(_ => _.IsSubclassOf(typeof(WorkflowStep))).ToList();
                    foreach (var ext in exts)
                    {
                        _availableExtensionTypes.TryAdd(ext.FullName, ext);
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"An exception occured while loading the assembly '{assemblyPath}'");
                }
            }
        }

        private void LoadRuntimeItems(IEnumerable<FrameworkSpecificGroup> runtimeItems, string packageDirectory)
        {
            var items = runtimeItems.SelectMany(frameworkGroup => frameworkGroup.Items.Select(runtimeItem =>
            {
                var libraryPath = Path.Combine(packageDirectory, runtimeItem);
                return new RuntimeItem(libraryPath, frameworkGroup.TargetFramework.Framework, frameworkGroup.TargetFramework.Profile);
            }));
            _runtimeItems.AddRange(items);
        }

        public bool TryResolveType(string name, out Type type)
        {
            if (_availableExtensionTypes.TryGetValue(name, out type))
                return true;

            return false;
        }

        public bool TryResolveRuntimeDependency(string libraryName, out string libraryPath)
        {
            libraryPath = string.Empty;

            var runtimeLibraries = _runtimeItems
                .Where(runtimeItem => string.Equals(libraryName, runtimeItem.LibraryName))
                .ToList();

            if (runtimeLibraries.Any())
            {
                if (runtimeLibraries.Count == 1)
                {
                    libraryPath = runtimeLibraries.First().LibraryPath;
                    return true;
                }
            }

            return false;
        }

    }

    internal class RuntimeItem
    {
        public string LibraryName { get; private set; }
        public string LibraryPath { get; private set; }
        public string? Platform { get; private set; }
        public string? Architecture { get; private set; }

        public RuntimeItem(string libraryPath, string? platform, string? architecture)
        {
            LibraryPath = libraryPath;
            LibraryName = Path.GetFileName(libraryPath);
            Platform = platform;
            Architecture = architecture;
        }
    }
}
