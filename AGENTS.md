# AGENTS.md — Auto-generated Task Instructions

## Current Task
**task-1 Add [MSBuildMultiThreadableTask] to simple stateless tasks in Microsoft.NET.Build.Tasks (batch 1)**

Add the `[MSBuildMultiThreadableTask]` attribute directly above the class declaration in these 16 task files that have no global state access and perform only in-memory data transforms: `CheckForTargetInAssetsFile.cs`, `CheckForUnsupportedWinMDReferences.cs`, `FilterResolvedFiles.cs`, `GenerateClsidMap.cs`, `GenerateRegFreeComManifest.cs`, `GenerateToolsSettingsFile.cs`, `GetAssemblyAttributes.cs`, `GetAssemblyVersion.cs`, `GetPackageDirectory.cs`, `SelectRuntimeIdentifierSpecificItems.cs`, `ShowPreviewMessage.cs`, `ValidateExecutableReferences.cs`, `SetGeneratedAppConfigMetadata.cs`, `AllowEmptyTelemetry.cs` (note: this file uses namespace `Microsoft.Build.Tasks`, not `Microsoft.NET.Build.Tasks` — this is fine, it compiles in the same project), `ParseTargetManifests.cs`, `ProduceContentAssets.cs`. The attribute is available via the `Microsoft.Build.Framework` namespace (on .NET Core from the package, on NETFRAMEWORK from the polyfill in `src/Tasks/Common/MSBuildMultiThreadableTaskAttribute.cs` which is already included via `<Compile Include="..\Common\**\*.cs" />`). Build with `.dotnet/dotnet build src/Tasks/Microsoft.NET.Build.Tasks/Microsoft.NET.Build.Tasks.csproj` to verify.

## Working Rules
- Complete the task described above
- Run tests before finishing
- Do not modify files outside the scope of this task
- Commit your changes with a clear commit message


