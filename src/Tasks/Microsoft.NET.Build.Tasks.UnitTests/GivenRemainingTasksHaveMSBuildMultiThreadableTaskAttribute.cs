// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;
using FluentAssertions;
using Microsoft.Build.Framework;
using Microsoft.Build.Tasks;
using Xunit;

namespace Microsoft.NET.Build.Tasks.UnitTests
{
    public class GivenRemainingTasksHaveMSBuildMultiThreadableTaskAttribute
    {
        /// <summary>
        /// Verifies that batch 3 remaining safe tasks in Microsoft.NET.Build.Tasks
        /// are decorated with [MSBuildMultiThreadableTask] so MSBuild can
        /// safely execute them in parallel.
        /// </summary>
        [Theory]
        [InlineData(typeof(AllowEmptyTelemetry))]
        [InlineData(typeof(CheckForTargetInAssetsFile))]
        [InlineData(typeof(CreateAppHost))]
        [InlineData(typeof(CreateComHost))]
        [InlineData(typeof(GenerateBundle))]
        [InlineData(typeof(GenerateRegFreeComManifest))]
        [InlineData(typeof(GenerateToolsSettingsFile))]
        [InlineData(typeof(GetAssemblyAttributes))]
        [InlineData(typeof(GetAssemblyVersion))]
        [InlineData(typeof(GetPackageDirectory))]
        [InlineData(typeof(ParseTargetManifests))]
        [InlineData(typeof(ProduceContentAssets))]
        [InlineData(typeof(ResolvePackageDependencies))]
        [InlineData(typeof(SelectRuntimeIdentifierSpecificItems))]
        [InlineData(typeof(ShowMissingWorkloads))]
        [InlineData(typeof(ShowPreviewMessage))]
        [InlineData(typeof(ValidateExecutableReferences))]
        public void TaskShouldHaveMSBuildMultiThreadableTaskAttribute(Type taskType)
        {
            taskType.GetCustomAttribute<MSBuildMultiThreadableTaskAttribute>()
                .Should().NotBeNull(
                    because: $"{taskType.Name} should be decorated with [MSBuildMultiThreadableTask] for parallel execution safety");
        }

        /// <summary>
        /// Verifies that the MSBuildMultiThreadableTask attribute has Inherited = false
        /// to ensure proper non-inheritable semantics.
        /// </summary>
        [Theory]
        [InlineData(typeof(AllowEmptyTelemetry))]
        [InlineData(typeof(CheckForTargetInAssetsFile))]
        [InlineData(typeof(CreateAppHost))]
        [InlineData(typeof(CreateComHost))]
        [InlineData(typeof(GenerateBundle))]
        [InlineData(typeof(GenerateRegFreeComManifest))]
        [InlineData(typeof(GenerateToolsSettingsFile))]
        [InlineData(typeof(GetAssemblyAttributes))]
        [InlineData(typeof(GetAssemblyVersion))]
        [InlineData(typeof(GetPackageDirectory))]
        [InlineData(typeof(ParseTargetManifests))]
        [InlineData(typeof(ProduceContentAssets))]
        [InlineData(typeof(ResolvePackageDependencies))]
        [InlineData(typeof(SelectRuntimeIdentifierSpecificItems))]
        [InlineData(typeof(ShowMissingWorkloads))]
        [InlineData(typeof(ShowPreviewMessage))]
        [InlineData(typeof(ValidateExecutableReferences))]
        public void MSBuildMultiThreadableTaskAttributeShouldNotBeInherited(Type taskType)
        {
            var attribute = taskType.GetCustomAttribute<MSBuildMultiThreadableTaskAttribute>();
            attribute.Should().NotBeNull();

            var attributeUsage = attribute!.GetType().GetCustomAttribute<AttributeUsageAttribute>();
            attributeUsage.Should().NotBeNull();
            attributeUsage!.Inherited.Should().BeFalse(
                "MSBuildMultiThreadableTask attribute must have Inherited = false");
        }
    }
}
