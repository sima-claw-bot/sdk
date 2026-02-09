// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;
using FluentAssertions;
using Microsoft.Build.Framework;
using Xunit;

namespace Microsoft.NET.Build.Tasks.UnitTests
{
    public class GivenMSBuildMultiThreadableTaskAttribute
    {
        /// <summary>
        /// Verifies that simple stateless tasks in Microsoft.NET.Build.Tasks
        /// are decorated with [MSBuildMultiThreadableTask] so MSBuild can
        /// safely execute them in parallel.
        /// </summary>
        [Theory]
        [InlineData(typeof(AddPackageType))]
        [InlineData(typeof(ApplyImplicitVersions))]
        [InlineData(typeof(CheckForDuplicateFrameworkReferences))]
        [InlineData(typeof(CheckForDuplicateItemMetadata))]
        [InlineData(typeof(CheckForDuplicateItems))]
        [InlineData(typeof(CheckForImplicitPackageReferenceOverrides))]
        [InlineData(typeof(CheckIfPackageReferenceShouldBeFrameworkReference))]
        [InlineData(typeof(CollatePackageDownloads))]
        [InlineData(typeof(CollectSDKReferencesDesignTime))]
        [InlineData(typeof(CreateWindowsSdkKnownFrameworkReferences))]
        [InlineData(typeof(FindItemsFromPackages))]
        [InlineData(typeof(GenerateGlobalUsings))]
        [InlineData(typeof(GenerateSupportedTargetFrameworkAlias))]
        [InlineData(typeof(GetDefaultPlatformTargetForNetFramework))]
        [InlineData(typeof(GetEmbeddedApphostPaths))]
        [InlineData(typeof(GetNuGetShortFolderName))]
        [InlineData(typeof(GetPublishItemsOutputGroupOutputs))]
        [InlineData(typeof(JoinItems))]
        [InlineData(typeof(RemoveDuplicatePackageReferences))]
        [InlineData(typeof(ResolveFrameworkReferences))]
        public void TaskShouldHaveMSBuildMultiThreadableTaskAttribute(Type taskType)
        {
            taskType
                .GetCustomAttributes()
                .Should()
                .ContainSingle(a => a.GetType().Name == "MSBuildMultiThreadableTaskAttribute",
                    $"{taskType.Name} should be decorated with [MSBuildMultiThreadableTask] for parallel execution safety");
        }

        [Theory]
        [InlineData(typeof(AddPackageType))]
        [InlineData(typeof(ApplyImplicitVersions))]
        [InlineData(typeof(CheckForDuplicateFrameworkReferences))]
        [InlineData(typeof(CheckForDuplicateItemMetadata))]
        [InlineData(typeof(CheckForDuplicateItems))]
        [InlineData(typeof(CheckForImplicitPackageReferenceOverrides))]
        [InlineData(typeof(CheckIfPackageReferenceShouldBeFrameworkReference))]
        [InlineData(typeof(CollatePackageDownloads))]
        [InlineData(typeof(CollectSDKReferencesDesignTime))]
        [InlineData(typeof(CreateWindowsSdkKnownFrameworkReferences))]
        [InlineData(typeof(FindItemsFromPackages))]
        [InlineData(typeof(GenerateGlobalUsings))]
        [InlineData(typeof(GenerateSupportedTargetFrameworkAlias))]
        [InlineData(typeof(GetDefaultPlatformTargetForNetFramework))]
        [InlineData(typeof(GetEmbeddedApphostPaths))]
        [InlineData(typeof(GetNuGetShortFolderName))]
        [InlineData(typeof(GetPublishItemsOutputGroupOutputs))]
        [InlineData(typeof(JoinItems))]
        [InlineData(typeof(RemoveDuplicatePackageReferences))]
        [InlineData(typeof(ResolveFrameworkReferences))]
        public void MSBuildMultiThreadableTaskAttributeShouldNotBeInherited(Type taskType)
        {
            var attribute = taskType
                .GetCustomAttributes()
                .Single(a => a.GetType().Name == "MSBuildMultiThreadableTaskAttribute");

            var attributeUsage = attribute.GetType().GetCustomAttribute<AttributeUsageAttribute>();
            attributeUsage.Should().NotBeNull();
            attributeUsage!.Inherited.Should().BeFalse(
                "MSBuildMultiThreadableTask attribute must have Inherited = false");
        }
    }
}
