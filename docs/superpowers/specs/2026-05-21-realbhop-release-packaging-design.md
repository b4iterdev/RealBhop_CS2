# RealBhop CS2 Release Packaging Design

## Overview
The current GitHub release artifact includes all `dotnet publish` output, which is larger than necessary for a CounterStrikeSharp plugin. This design defines a leaner release package that ships only the plugin and required runtime metadata, relying on CounterStrikeSharp’s shared dependencies already present on the server.

## Goals
- Reduce the size of the GitHub release zip while keeping runtime behavior unchanged.
- Keep the installation process simple for server operators.

## Non-Goals
- Changing plugin runtime behavior or configuration.
- Introducing new build tooling beyond minimal packaging steps.

## Recommended Approach
Package a **plugin-only** release:

### Package Contents
- `RealBhopCS2.dll`
- `RealBhopCS2.deps.json` (only if required by CounterStrikeSharp at runtime)
- `RealBhopCS2.runtimeconfig.json` (only if required by CounterStrikeSharp at runtime)
- Any default plugin config files, if present

Exclude framework and shared dependency DLLs (e.g., `Microsoft.Extensions.*`, `Serilog.*`) that are already provided by CounterStrikeSharp or the server environment.

## Build & Release Workflow
1. Build as usual (`dotnet publish` or `dotnet build`) to produce the plugin assembly.
2. Copy only the package contents above into a release folder.
3. Zip the release folder and upload as the GitHub release asset.

## Validation
- Install the zip into `addons/counterstrikesharp/plugins/RealBhopCS2`.
- Restart the server.
- Confirm the plugin loads and the `css_realbhop_status` command works.

## Risks & Mitigations
- **Risk:** Missing a required runtime file.
  - **Mitigation:** Validate on a clean server with only CounterStrikeSharp installed. If a missing dependency is detected, add it to the package allowlist.

## Success Criteria
- Release zip size is materially smaller than the current publish artifact.
- Plugin loads without errors and commands function as expected.
