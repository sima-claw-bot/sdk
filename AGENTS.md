# AGENTS.md — Auto-generated Task Instructions

## Current Task
**task-2 Add [MSBuildMultiThreadableTask] to file-I/O tasks in Microsoft.NET.Build.Tasks (batch 2)**

Add the attribute to these 11 tasks that perform file I/O using only absolute paths from task properties (thread-safe): `CreateAppHost.cs`, `CreateComHost.cs`, `GenerateBundle.cs`, `GenerateRuntimeConfigurationFiles.cs`, `GenerateShims.cs`, `PrepareForReadyToRunCompilation.cs`, `ResolveCopyLocalAssets.cs`, `GetPackagesToPrune.cs` (uses static readonly dictionaries from FrameworkPackages — these are immutable after type initialization and safe for concurrent reads), `ResolveAppHosts.cs`, `WriteAppConfigWithSupportedRuntime.cs`, `ResolvePackageDependencies.cs`. Verify the project builds.

## Working Rules
- Complete the task described above
- Run tests before finishing
- Do not modify files outside the scope of this task
- Commit your changes with a clear commit message


