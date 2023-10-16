﻿using Flowly.Core.Providers;
using NuGet.Configuration;
using System.Collections.Generic;

namespace Flowly.ExtensionSource.NuGet
{
    public class NuGetExtensionSource : IExtensionSource
    {
        static PackageSource[] DefaultPackageSources = new[]
        {
            new PackageSource("https://api.nuget.org/v3/index.json")
        };

        public List<PackageSource> PackageSources { get; set; } = new List<PackageSource>();


        public IExtensionProvider BuildProvider()
        {
            var sources = new List<PackageSource>();
            sources.AddRange(DefaultPackageSources);
            sources.AddRange(PackageSources);

            return new NuGetExtensionProvider(sources.ToArray());
        }
    }
}
