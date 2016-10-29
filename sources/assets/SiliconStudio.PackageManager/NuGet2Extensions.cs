﻿// Copyright (c) 2016 Silicon Studio Corp. (http://siliconstudio.co.jp)
// This file is distributed under GPL v3. See LICENSE.md for details.

namespace SiliconStudio.PackageManager
{
    internal static class NuGet2Extensions
    {
        /// <summary>
        /// Given a <see cref="ConstraintProvider"/> construct a NuGet equivalent.
        /// </summary>
        /// <param name="provider">The provider to convert.</param>
        /// <returns>An instance of conforming type <see cref="IPackageConstraintProvider"/> matching <paramref name="provider"/>.</returns>
        public static NuGet.IPackageConstraintProvider Provider (this ConstraintProvider provider)
        {
            if ((provider == null) || (!provider.HasConstraints))
            {
                return NuGet.NullConstraintProvider.Instance;
            }
            else
            {
                var res = new NuGet.DefaultConstraintProvider();
                foreach (var constraint in provider.Constraints)
                {
                    res.AddConstraint(constraint.Key, constraint.Value.ToVersionSpec());
                }
                return res;
            }
        }

        public static PackageVersionRange ToPackageVersionRange(this NuGet.IVersionSpec version)
        {
            PackageVersion min = null, max = null;
            if (version.MinVersion?.Version != null)
            {
                min = new PackageVersion(version.MinVersion.Version);
            }
            if (version.MaxVersion?.Version != null)
            {
                max = new PackageVersion(version.MaxVersion.Version);
            }
            return new PackageVersionRange(min, version.IsMinInclusive, max, version.IsMaxInclusive);
        }


        public static NuGet.VersionSpec ToVersionSpec(this PackageVersionRange range)
        {
            return new NuGet.VersionSpec()
            {
                MinVersion = range.MinVersion != null ? range.MinVersion.ToSemanticVersion().SemanticVersion : null,
                IsMinInclusive = range.IsMinInclusive,
                MaxVersion = range.MaxVersion != null ? range.MaxVersion.ToSemanticVersion().SemanticVersion : null,
                IsMaxInclusive = range.IsMaxInclusive
            };
        }

        public static NuGet.ManifestFile ToManifestFile(this ManifestFile file)
        {
            return new NuGet.ManifestFile()
            {
                Source = file.Source,
                Exclude = file.Exclude,
                Target = file.Target
            };
        }

        public static NuGet.ManifestMetadata ToManifestMetadata(this ManifestMetadata meta)
        {
            var nugetMeta = new NuGet.ManifestMetadata()
            {
                Id = meta.Id,
                Authors = meta.Authors,
                Description = meta.Description,
                Copyright = meta.Copyright,
                DevelopmentDependency = meta.DevelopmentDependency,
                Version = meta.Version,
                Owners = meta.Owners,
                IconUrl = meta.IconUrl,
                Language = meta.Language,
                LicenseUrl = meta.LicenseUrl,
                MinClientVersionString = meta.MinClientVersionString,
                ProjectUrl = meta.ProjectUrl,
                ReleaseNotes = meta.ReleaseNotes,
                RequireLicenseAcceptance = meta.RequireLicenseAcceptance,
                Summary = meta.Summary,
                Tags = meta.Tags,
                Title = meta.Title
            };
            // Copy list of dependencies in the first slot of ManifestDependencySet, and create
            // it if it doesn't exist
            NuGet.ManifestDependencySet dependencySet;
            if (nugetMeta.DependencySets.Count == 0)
            {
                dependencySet = new NuGet.ManifestDependencySet();
                nugetMeta.DependencySets.Add(dependencySet);
            }
            else
            {
                dependencySet = nugetMeta.DependencySets[0];
            }
            foreach (var deps in meta.Dependencies)
            {
                dependencySet.Dependencies.Add(new NuGet.ManifestDependency() { Id = deps.Id, Version = deps.Version });
            }

            return nugetMeta;
        }
    }
}
