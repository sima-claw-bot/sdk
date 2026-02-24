// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using Microsoft.Build.Framework;
using NuGet.ProjectModel;

namespace Microsoft.NET.Build.Tasks
{
    [MSBuildMultiThreadableTask]
    public class CheckForTargetInAssetsFile : TaskBase, IMultiThreadableTask
    {
        public TaskEnvironment TaskEnvironment { get; set; }

        public string AssetsFilePath { get; set; }

        [Required]
        public string TargetFramework { get; set; }

        public string RuntimeIdentifier { get; set; }


        protected override void ExecuteCore()
        {
            string assetsFilePath = TaskEnvironment?.GetAbsolutePath(AssetsFilePath) ?? AssetsFilePath;
            LockFile lockFile = new LockFileCache(this).GetLockFile(assetsFilePath);

            lockFile.GetTargetAndThrowIfNotFound(TargetFramework, RuntimeIdentifier);
        }
    }
}
