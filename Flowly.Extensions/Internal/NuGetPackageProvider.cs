using Flowly.Core.Definitions;
using NuGet.Common;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using Microsoft.Extensions.DependencyModel;
using NuGet.Frameworks;
using NuGet.Configuration;
using NuGet.Packaging.Signing;
using NuGet.Packaging;
using NuGet.Resolver;
using System.IO;

namespace Flowly.Extensions.Core.Internal
{
    internal static class NuGetPackageProvider
    {
        public static IEnumerable<SourcePackageDependencyInfo> GetPackagesToInstall(SourceRepositoryProvider sourceRepositoryProvider,
                                                                      ILogger logger, IEnumerable<ExtensionDefinition> extensions,
                                                                      HashSet<SourcePackageDependencyInfo> allPackages)
        {
            // Create a package resolver context.
            var resolverContext = new PackageResolverContext(
                    DependencyBehavior.Lowest,
                    extensions.Select(x => x.Package),
                    Enumerable.Empty<string>(),
                    Enumerable.Empty<PackageReference>(),
                    Enumerable.Empty<PackageIdentity>(),
                    allPackages,
                    sourceRepositoryProvider.GetRepositories().Select(s => s.PackageSource),
                    logger);

            var resolver = new PackageResolver();

            // Work out the actual set of packages to install.
            var packagesToInstall = resolver.Resolve(resolverContext, CancellationToken.None)
                                            .Select(p => allPackages.Single(x => PackageIdentityComparer.Default.Equals(x, p)));
            return packagesToInstall;
        }

        public static async Task InstallPackages(SourceCacheContext sourceCacheContext, ILogger logger,
                                    IEnumerable<SourcePackageDependencyInfo> packagesToInstall, string rootPackagesDirectory,
                                    ISettings nugetSettings, CancellationToken cancellationToken)
        {
            var packagePathResolver = new PackagePathResolver(rootPackagesDirectory, true);
            var packageExtractionContext = new PackageExtractionContext(
                PackageSaveMode.Defaultv3,
                XmlDocFileSaveMode.Skip,
                ClientPolicyContext.GetClientPolicy(nugetSettings, logger),
                logger);

            foreach (var package in packagesToInstall)
            {
                var downloadResource = await package.Source.GetResourceAsync<DownloadResource>(cancellationToken);

                // Download the package (might come from the shared package cache).
                var downloadResult = await downloadResource.GetDownloadResourceResultAsync(
                    package,
                    new PackageDownloadContext(sourceCacheContext),
                    SettingsUtility.GetGlobalPackagesFolder(nugetSettings),
                    logger,
                    cancellationToken);

                // Extract the package into the target directory.
                await PackageExtractor.ExtractPackageAsync(
                    downloadResult.PackageSource,
                    downloadResult.PackageStream,
                    packagePathResolver,
                    packageExtractionContext,
                    cancellationToken);
            }
        }

        public static async Task GetPackageDependencies(PackageIdentity package, SourceCacheContext cacheContext,
                                          NuGetFramework framework, ILogger logger,
                                          IEnumerable<SourceRepository> repositories,
                                          DependencyContext hostDependencies,
                                          ISet<SourcePackageDependencyInfo> availablePackages,
                                          CancellationToken cancelToken)
        {
            // Don't recurse over a package we've already seen.
            if (availablePackages.Contains(package))
            {
                return;
            }

            foreach (var sourceRepository in repositories)
            {
                // Get the dependency info for the package.
                var dependencyInfoResource = await sourceRepository.GetResourceAsync<DependencyInfoResource>();
                var dependencyInfo = await dependencyInfoResource.ResolvePackage(
                    package,
                    framework,
                    cacheContext,
                    logger,
                    cancelToken);

                // No info for the package in this repository.
                if (dependencyInfo == null)
                {
                    continue;
                }

                // Filter the dependency info.
                // Don't bring in any dependencies that are provided by the host.
                var actualSourceDep = new SourcePackageDependencyInfo(
                    dependencyInfo.Id,
                    dependencyInfo.Version,
                    dependencyInfo.Dependencies.Where(dep => !DependencySuppliedByHost(hostDependencies, dep)),
                    dependencyInfo.Listed,
                    dependencyInfo.Source);

                // Add to the list of all packages.
                availablePackages.Add(dependencyInfo);

                // Recurse through each package.
                foreach (var dependency in dependencyInfo.Dependencies)
                {
                    await GetPackageDependencies(
                        new PackageIdentity(dependency.Id, dependency.VersionRange.MinVersion),
                        cacheContext,
                        framework,
                        logger,
                        repositories,
                        hostDependencies,
                        availablePackages,
                        cancelToken);
                }

                break;
            }
        }

        public static async Task<PackageIdentity> GetPackageIdentity(
          ExtensionDefinition extConfig, SourceCacheContext cache, ILogger nugetLogger,
          IEnumerable<SourceRepository> repositories, CancellationToken cancelToken)
        {
            foreach (var sourceRepository in repositories)
            {
                // Get a 'resource' from the repository.
                var findPackageResource = await sourceRepository.GetResourceAsync<FindPackageByIdResource>();

                // Get the list of all available versions of the package in the repository.
                var allVersions = await findPackageResource.GetAllVersionsAsync(extConfig.Package, cache, nugetLogger, cancelToken);

                NuGetVersion selected;

                // Have we specified a version range?
                if (extConfig.Version != null)
                {
                    if (!VersionRange.TryParse(extConfig.Version, out var range))
                    {
                        throw new InvalidOperationException("Invalid version range provided.");
                    }

                    // Find the best package version match for the range.
                    // Consider pre-release versions, but only if the extension is configured to use them.
                    var bestVersion = range.FindBestMatch(allVersions.Where(v => extConfig.PreRelease || !v.IsPrerelease));

                    selected = bestVersion;
                }
                else
                {
                    // No version; choose the latest, allow pre-release if configured.
                    selected = allVersions.LastOrDefault(v => v.IsPrerelease == extConfig.PreRelease);
                }

                if (selected is object)
                {
                    return new PackageIdentity(extConfig.Package, selected);
                }
            }

            return null;
        }

        public static void LoadPackageAssemblies(IEnumerable<SourcePackageDependencyInfo> packagesToInstall, string rootPackagesDirectory,
                                    ISettings nugetSettings, NuGetFramework targetFramework, CancellationToken cancellationToken)
        {
            

            foreach (var item in packagesToInstall)
            {
                var id = item.Id;

                var version = item.Version;
                var folderName = $"{item.Id}.{item.Version}";
                var absPath = Path.Combine(rootPackagesDirectory, folderName, "lib");
                var packageFrameworks = Directory
                    .EnumerateDirectories(absPath)
                    .Select(libFolderPath => new FrameworkFromLibFolderPath(libFolderPath))
                    .ToList();
                                var path = packageFrameworks.Count > 0 ? packageFrameworks.GetNearest(targetFramework)?.LibFolderPath ?? absPath : absPath;
                int x = 0;
            }
        }

        private static bool DependencySuppliedByHost(DependencyContext hostDependencies, PackageDependency dep)
        {
            // See if a runtime library with the same ID as the package is available in the host's runtime libraries.
            var runtimeLib = hostDependencies.RuntimeLibraries.FirstOrDefault(r => r.Name == dep.Id);

            if (runtimeLib is object)
            {
                // What version of the library is the host using?
                var parsedLibVersion = NuGetVersion.Parse(runtimeLib.Version);

                if (parsedLibVersion.IsPrerelease)
                {
                    // Always use pre-release versions from the host, otherwise it becomes
                    // a nightmare to develop across multiple active versions.
                    return true;
                }
                else
                {
                    // Does the host version satisfy the version range of the requested package?
                    // If so, we can provide it; otherwise, we cannot.
                    return dep.VersionRange.Satisfies(parsedLibVersion);
                }
            }

            return false;
        }
    }

    public class FrameworkFromLibFolderPath : IFrameworkSpecific
    {
        public FrameworkFromLibFolderPath(string libFolderPath)
        {
            LibFolderPath = libFolderPath;
            TargetFramework = NuGetFramework.ParseFolder(Path.GetFileName(libFolderPath));
        }

        public readonly string LibFolderPath;

        public NuGetFramework TargetFramework { get; }
    }
}
