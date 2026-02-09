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
        /// Verifies that file-I/O tasks in Microsoft.NET.Build.Tasks (batch 2)
        /// are decorated with [MSBuildMultiThreadableTask] so MSBuild can
        /// safely execute them in parallel.
        /// </summary>
        [Theory]
        [InlineData(typeof(CreateAppHost))]
        [InlineData(typeof(CreateComHost))]
        [InlineData(typeof(GenerateBundle))]
        [InlineData(typeof(GenerateRuntimeConfigurationFiles))]
        [InlineData(typeof(GenerateShims))]
        [InlineData(typeof(GetPackagesToPrune))]
        [InlineData(typeof(PrepareForReadyToRunCompilation))]
        [InlineData(typeof(ResolveAppHosts))]
        [InlineData(typeof(ResolveCopyLocalAssets))]
        [InlineData(typeof(ResolvePackageDependencies))]
        [InlineData(typeof(WriteAppConfigWithSupportedRuntime))]
        public void FileIOTaskShouldHaveMSBuildMultiThreadableTaskAttribute(Type taskType)
        {
            taskType
                .GetCustomAttributes()
                .Should()
                .ContainSingle(a => a.GetType().Name == "MSBuildMultiThreadableTaskAttribute",
                    $"{taskType.Name} should be decorated with [MSBuildMultiThreadableTask] for parallel execution safety");
        }

        [Theory]
        [InlineData(typeof(CreateAppHost))]
        [InlineData(typeof(CreateComHost))]
        [InlineData(typeof(GenerateBundle))]
        [InlineData(typeof(GenerateRuntimeConfigurationFiles))]
        [InlineData(typeof(GenerateShims))]
        [InlineData(typeof(GetPackagesToPrune))]
        [InlineData(typeof(PrepareForReadyToRunCompilation))]
        [InlineData(typeof(ResolveAppHosts))]
        [InlineData(typeof(ResolveCopyLocalAssets))]
        [InlineData(typeof(ResolvePackageDependencies))]
        [InlineData(typeof(WriteAppConfigWithSupportedRuntime))]
        public void FileIOTaskMSBuildMultiThreadableTaskAttributeShouldNotBeInherited(Type taskType)
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
