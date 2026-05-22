# RealBhop CS2 External Config Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add CounterStrikeSharp-native external config support, a runtime reload command, and temporary in-memory debug toggling.

**Architecture:** `RealBhopConfig` becomes a `BasePluginConfig`, and `RealBhopPlugin` implements `IPluginConfig<RealBhopConfig>`. A small runtime state object owns the temporary debug override so file-backed `Debug` values are not mutated by `css_realbhop debug`.

**Tech Stack:** .NET 8, CounterStrikeSharp API, xUnit.

---

## File Structure
- Modify: `src/RealBhopCS2/Config/RealBhopConfig.cs` — inherit from CounterStrikeSharp config base.
- Create: `src/RealBhopCS2/Config/RealBhopRuntimeConfig.cs` — wrap file config and runtime debug override.
- Modify: `src/RealBhopCS2/RealBhopPlugin.cs` — implement `IPluginConfig<RealBhopConfig>`, register reload command, use runtime config.
- Modify: `src/RealBhopCS2/Commands/RealBhopCommands.cs` — status/debug/reload command handlers.
- Modify: `src/RealBhopCS2/Diagnostics/RealBhopDebugFormatter.cs` — display file-backed and effective debug state.
- Create: `tests/RealBhopCS2.Tests/RealBhopRuntimeConfigTests.cs` — test temporary debug override behavior.
- Modify: `tests/RealBhopCS2.Tests/RealBhopDebugFormatterTests.cs` — test updated status format.
- Modify: `README.md` — document config file path and commands.

---

### Task 1: Add runtime config wrapper

**Files:**
- Modify: `src/RealBhopCS2/Config/RealBhopConfig.cs`
- Create: `src/RealBhopCS2/Config/RealBhopRuntimeConfig.cs`
- Create: `tests/RealBhopCS2.Tests/RealBhopRuntimeConfigTests.cs`

- [ ] Write tests proving effective debug defaults to config value, toggling debug does not mutate `RealBhopConfig.Debug`, and reload can preserve the override.
- [ ] Make `RealBhopConfig` inherit `BasePluginConfig`.
- [ ] Implement `RealBhopRuntimeConfig` with `Config`, `EffectiveDebug`, `ToggleDebug()`, and `Reload(Func<RealBhopConfig> reload)`.
- [ ] Run targeted tests.

### Task 2: Wire plugin and commands

**Files:**
- Modify: `src/RealBhopCS2/RealBhopPlugin.cs`
- Modify: `src/RealBhopCS2/Commands/RealBhopCommands.cs`

- [ ] Implement `IPluginConfig<RealBhopConfig>` in `RealBhopPlugin`.
- [ ] Use `OnConfigParsed` to initialize runtime config before `Load`.
- [ ] Pass runtime config to commands and effective file config to tracker/movement.
- [ ] Add `css_realbhop reload` command that calls CounterStrikeSharp `Config.Reload()` and preserves temporary debug override.
- [ ] Keep `css_realbhop debug` temporary and never call `Config.Update()`.

### Task 3: Update status formatting and docs

**Files:**
- Modify: `src/RealBhopCS2/Diagnostics/RealBhopDebugFormatter.cs`
- Modify: `tests/RealBhopCS2.Tests/RealBhopDebugFormatterTests.cs`
- Modify: `README.md`

- [ ] Update status output to include effective debug and config debug.
- [ ] Update formatter test assertions.
- [ ] Document config path, `css_realbhop reload`, and temporary `css_realbhop debug` behavior.

### Task 4: Verify

**Files:**
- All changed files.

- [ ] Run LSP diagnostics on changed C# files.
- [ ] Run `dotnet test RealBhopCS2.sln --configuration Release`.
- [ ] Run `dotnet build RealBhopCS2.sln --configuration Release --no-restore`.

## Self-Review
- Spec coverage: all approved requirements map to Tasks 1-4.
- Placeholder scan: no placeholder requirements remain.
- Type consistency: `RealBhopRuntimeConfig` owns the debug override and exposes effective runtime state to commands/plugin.
