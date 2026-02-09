// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;
using FluentAssertions;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Xunit;

namespace Microsoft.NET.Build.Tasks.UnitTests
{
    public class GivenMultiThreadableTaskAttributes
    {
        private static readonly Assembly TaskAssembly = typeof(TaskBase).Assembly;

        /// <summary>
        /// All task types in Microsoft.NET.Build.Tasks that derive from Task or TaskBase.
        /// Handles ReflectionTypeLoadException for types with missing dependencies.
        /// </summary>
        private static IEnumerable<Type> GetAllTaskTypes()
        {
            Type[] types;
            try
            {
                types = TaskAssembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types.Where(t => t != null).ToArray()!;
            }

            return types
                .Where(t => t.IsClass && !t.IsAbstract && t.IsPublic)
                .Where(t => typeof(Microsoft.Build.Utilities.Task).IsAssignableFrom(t));
        }

        /// <summary>
        /// Returns all task types that are marked with [MSBuildMultiThreadableTask].
        /// </summary>
        private static IEnumerable<Type> GetMultiThreadableTaskTypes()
        {
            return GetAllTaskTypes()
                .Where(t => t.GetCustomAttributes()
                    .Any(a => a.GetType().Name == "MSBuildMultiThreadableTaskAttribute"));
        }

        /// <summary>
        /// The expected set of tasks that should have the [MSBuildMultiThreadableTask] attribute.
        /// </summary>
        public static IEnumerable<object[]> ExpectedMultiThreadableTasks => new List<object[]>
        {
            new object[] { typeof(AddPackageType) },
            new object[] { typeof(ApplyImplicitVersions) },
            new object[] { typeof(CheckForDuplicateFrameworkReferences) },
            new object[] { typeof(CheckForDuplicateItemMetadata) },
            new object[] { typeof(CheckForDuplicateItems) },
            new object[] { typeof(CheckForImplicitPackageReferenceOverrides) },
            new object[] { typeof(CheckIfPackageReferenceShouldBeFrameworkReference) },
            new object[] { typeof(CollatePackageDownloads) },
            new object[] { typeof(CollectSDKReferencesDesignTime) },
            new object[] { typeof(CreateWindowsSdkKnownFrameworkReferences) },
            new object[] { typeof(FindItemsFromPackages) },
            new object[] { typeof(GenerateGlobalUsings) },
            new object[] { typeof(GenerateSupportedTargetFrameworkAlias) },
            new object[] { typeof(GetDefaultPlatformTargetForNetFramework) },
            new object[] { typeof(GetEmbeddedApphostPaths) },
            new object[] { typeof(GetNuGetShortFolderName) },
            new object[] { typeof(GetPublishItemsOutputGroupOutputs) },
            new object[] { typeof(JoinItems) },
            new object[] { typeof(RemoveDuplicatePackageReferences) },
            new object[] { typeof(ResolveFrameworkReferences) },
        };

        [Theory]
        [MemberData(nameof(ExpectedMultiThreadableTasks))]
        public void ExpectedTasksHaveMultiThreadableAttribute(Type taskType)
        {
            var hasAttribute = taskType.GetCustomAttributes()
                .Any(a => a.GetType().Name == "MSBuildMultiThreadableTaskAttribute");

            hasAttribute.Should().BeTrue(
                $"'{taskType.Name}' should be marked with [MSBuildMultiThreadableTask] for thread-safe execution");
        }

        [Fact]
        public void MultiThreadableAttributeIsNotInheritable()
        {
            // The attribute should have Inherited = false so subclasses don't accidentally
            // inherit thread-safety designation without explicit review.
            Type? attrType;
            try
            {
                attrType = TaskAssembly.GetTypes()
                    .FirstOrDefault(t => t.Name == "MSBuildMultiThreadableTaskAttribute");
            }
            catch (ReflectionTypeLoadException ex)
            {
                attrType = ex.Types.Where(t => t != null)
                    .FirstOrDefault(t => t!.Name == "MSBuildMultiThreadableTaskAttribute");
            }

            // On .NET Core, the attribute comes from Microsoft.Build.Framework
            if (attrType == null)
            {
                attrType = typeof(MSBuildMultiThreadableTaskAttribute);
            }

            var usage = attrType.GetCustomAttribute<AttributeUsageAttribute>();
            usage.Should().NotBeNull("MSBuildMultiThreadableTaskAttribute should have [AttributeUsage]");
            usage!.Inherited.Should().BeFalse("MSBuildMultiThreadableTaskAttribute must not be inheritable");
        }

        [Fact]
        public void MultiThreadableAttributeDoesNotAllowMultiple()
        {
            Type? attrType;
            try
            {
                attrType = TaskAssembly.GetTypes()
                    .FirstOrDefault(t => t.Name == "MSBuildMultiThreadableTaskAttribute");
            }
            catch (ReflectionTypeLoadException ex)
            {
                attrType = ex.Types.Where(t => t != null)
                    .FirstOrDefault(t => t!.Name == "MSBuildMultiThreadableTaskAttribute");
            }

            if (attrType == null)
            {
                attrType = typeof(MSBuildMultiThreadableTaskAttribute);
            }

            var usage = attrType.GetCustomAttribute<AttributeUsageAttribute>();
            usage.Should().NotBeNull();
            usage!.AllowMultiple.Should().BeFalse("MSBuildMultiThreadableTaskAttribute should not allow multiple instances");
        }

        [Fact]
        public void MultiThreadableTasksShouldNotHaveStaticMutableFields()
        {
            // Tasks marked as multi-threadable should not have mutable static fields,
            // as these could cause race conditions in concurrent execution.
            var issues = new List<string>();

            foreach (var taskType in GetMultiThreadableTaskTypes())
            {
                var staticFields = taskType.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(f => !f.IsLiteral)   // Exclude constants
                    .Where(f => !f.IsInitOnly)   // Exclude readonly fields
                    .ToList();

                foreach (var field in staticFields)
                {
                    issues.Add($"{taskType.Name}.{field.Name} is a mutable static field");
                }
            }

            issues.Should().BeEmpty(
                "tasks marked with [MSBuildMultiThreadableTask] should not have mutable static fields " +
                "because they may be executed concurrently");
        }

        [Fact]
        public void MultiThreadableTasksShouldNotHaveStaticMutableProperties()
        {
            // Static properties with setters in multi-threadable tasks are a thread-safety concern.
            var issues = new List<string>();

            foreach (var taskType in GetMultiThreadableTaskTypes())
            {
                var staticProperties = taskType.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(p => p.CanWrite && p.GetSetMethod(nonPublic: true) != null)
                    .ToList();

                foreach (var prop in staticProperties)
                {
                    issues.Add($"{taskType.Name}.{prop.Name} is a mutable static property");
                }
            }

            issues.Should().BeEmpty(
                "tasks marked with [MSBuildMultiThreadableTask] should not have mutable static properties " +
                "because they may be executed concurrently");
        }

        [Fact]
        public void AllMultiThreadableTasksAreInExpectedList()
        {
            // Ensures new tasks marked with the attribute are also added to the expected list
            // so they get full test coverage.
            var expectedTypeNames = ExpectedMultiThreadableTasks
                .Select(args => ((Type)args[0]).Name)
                .ToHashSet();

            var actualMarkedTypes = GetMultiThreadableTaskTypes()
                .Select(t => t.Name)
                .ToHashSet();

            var missingFromExpected = actualMarkedTypes.Except(expectedTypeNames).ToList();

            missingFromExpected.Should().BeEmpty(
                "all tasks marked with [MSBuildMultiThreadableTask] should be listed in " +
                "ExpectedMultiThreadableTasks so they get thread-safety validation. " +
                "Missing: {0}", string.Join(", ", missingFromExpected));
        }

        [Fact]
        public async System.Threading.Tasks.Task ConcurrentExecutionOfCheckForDuplicateItems()
        {
            // Verify that a multi-threadable task can execute concurrently without issues.
            const int concurrency = 10;
            var tasks = new System.Threading.Tasks.Task<bool>[concurrency];

            for (int i = 0; i < concurrency; i++)
            {
                int index = i;
                tasks[i] = System.Threading.Tasks.Task.Run(() =>
                {
                    var items = new[]
                    {
                        new TaskItem($"file{index}.cs"),
                        new TaskItem($"other{index}.cs"),
                    };

                    var task = new CheckForDuplicateItems()
                    {
                        Items = items,
                        ItemName = "Compile",
                        PropertyNameToDisableDefaultItems = "EnableDefaultCompileItems",
                        MoreInformationLink = "https://aka.ms/sdkimplicitrefs",
                        DefaultItemsEnabled = true,
                        DefaultItemsOfThisTypeEnabled = true
                    };

                    return task.Execute();
                });
            }

            var results = await System.Threading.Tasks.Task.WhenAll(tasks);

            foreach (var result in results)
            {
                result.Should().BeTrue("each concurrent execution should succeed independently");
            }
        }

        [Fact]
        public async System.Threading.Tasks.Task ConcurrentExecutionOfRemoveDuplicatePackageReferences()
        {
            const int concurrency = 10;
            var tasks = new System.Threading.Tasks.Task<(bool success, int count)>[concurrency];

            for (int i = 0; i < concurrency; i++)
            {
                int index = i;
                tasks[i] = System.Threading.Tasks.Task.Run(() =>
                {
                    var items = new ITaskItem[]
                    {
                        new MockTaskItem($"Package.{index}.A", new Dictionary<string, string> { { "Version", "1.0.0" } }),
                        new MockTaskItem($"Package.{index}.B", new Dictionary<string, string> { { "Version", "2.0.0" } }),
                        new MockTaskItem($"Package.{index}.A", new Dictionary<string, string> { { "Version", "1.0.0" } }),
                    };

                    var task = new RemoveDuplicatePackageReferences()
                    {
                        InputPackageReferences = items,
                    };

                    bool success = task.Execute();
                    return (success, task.UniquePackageReferences.Length);
                });
            }

            var results = await System.Threading.Tasks.Task.WhenAll(tasks);

            foreach (var r in results)
            {
                r.success.Should().BeTrue("each concurrent execution should succeed");
                r.count.Should().Be(2, "duplicates should be removed independently in each execution");
            }
        }

        [Fact]
        public async System.Threading.Tasks.Task ConcurrentExecutionOfCollatePackageDownloads()
        {
            const int concurrency = 10;
            var tasks = new System.Threading.Tasks.Task<(bool success, int count)>[concurrency];

            for (int i = 0; i < concurrency; i++)
            {
                int index = i;
                tasks[i] = System.Threading.Tasks.Task.Run(() =>
                {
                    var packages = new ITaskItem[]
                    {
                        CreatePackageItem($"Pkg.{index}", "1.0.0"),
                        CreatePackageItem($"Pkg.{index}", "2.0.0"),
                        CreatePackageItem($"Other.{index}", "3.0.0"),
                    };

                    var task = new CollatePackageDownloads()
                    {
                        Packages = packages,
                    };

                    bool success = task.Execute();
                    return (success, task.PackageDownloads.Length);
                });
            }

            var results = await System.Threading.Tasks.Task.WhenAll(tasks);

            foreach (var r in results)
            {
                r.success.Should().BeTrue();
                r.count.Should().Be(2, "packages should be grouped by identity");
            }
        }

        [Fact]
        public async System.Threading.Tasks.Task ConcurrentExecutionOfGenerateGlobalUsings()
        {
            const int concurrency = 10;
            var tasks = new System.Threading.Tasks.Task<(bool success, int lineCount)>[concurrency];

            for (int i = 0; i < concurrency; i++)
            {
                int index = i;
                tasks[i] = System.Threading.Tasks.Task.Run(() =>
                {
                    var usings = new ITaskItem[]
                    {
                        new TaskItem($"System.Namespace{index}A"),
                        new TaskItem($"System.Namespace{index}B"),
                    };

                    var task = new GenerateGlobalUsings()
                    {
                        Usings = usings,
                    };

                    bool success = task.Execute();
                    return (success, task.Lines?.Length ?? 0);
                });
            }

            var results = await System.Threading.Tasks.Task.WhenAll(tasks);

            foreach (var r in results)
            {
                r.success.Should().BeTrue();
                // 1 header line + 2 using lines
                r.lineCount.Should().Be(3, "should produce header + 2 using lines");
            }
        }

        [Fact]
        public async System.Threading.Tasks.Task ConcurrentExecutionOfAddPackageType()
        {
            const int concurrency = 10;
            var tasks = new System.Threading.Tasks.Task<(bool success, string[]? result)>[concurrency];

            for (int i = 0; i < concurrency; i++)
            {
                int index = i;
                tasks[i] = System.Threading.Tasks.Task.Run(() =>
                {
                    var task = new AddPackageType()
                    {
                        CurrentPackageType = $"ExistingType{index}",
                        PackageTypeToAdd = $"NewType{index}",
                    };

                    bool success = task.Execute();
                    return (success, task.UpdatedPackageType);
                });
            }

            var results = await System.Threading.Tasks.Task.WhenAll(tasks);

            for (int i = 0; i < concurrency; i++)
            {
                results[i].success.Should().BeTrue();
                results[i].result.Should().Contain($"NewType{i}");
                results[i].result.Should().Contain($"ExistingType{i}");
            }
        }

        private static ITaskItem CreatePackageItem(string id, string version)
        {
            var item = new TaskItem(id);
            item.SetMetadata("Version", version);
            return item;
        }
    }
}
