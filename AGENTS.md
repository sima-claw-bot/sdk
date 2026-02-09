# AGENTS.md — Auto-generated Task Instructions

## Current Task
**task-4 Add [MSBuildMultiThreadableTask] to conflict resolution tasks in Common**

Add the attribute to `src/Tasks/Common/ConflictResolution/ResolvePackageFileConflicts.cs` (class `ResolvePackageFileConflicts : TaskBase`, uses only instance HashSet fields, no global state) and `src/Tasks/Common/ConflictResolution/ResolvePublishOutputConflicts.cs` (class `ResolveOverlappingItemGroupConflicts : TaskBase`, uses only local variables, no global state). These are compiled into Microsoft.NET.Build.Tasks via the `<Compile Include="..\Common\**\*.cs" />` wildcard. Verify the project builds.

## Working Rules
- Complete the task described above
- Run tests before finishing
- Do not modify files outside the scope of this task
- Commit your changes with a clear commit message


