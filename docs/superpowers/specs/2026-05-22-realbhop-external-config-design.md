# RealBhop CS2 External Config Design

## Overview
RealBhop CS2 currently uses hardcoded runtime defaults from `RealBhopConfig`. Server operators must rebuild the plugin to change most behavior. This design moves those settings into CounterStrikeSharp's native plugin config system and adds a reload command, while keeping `css_realbhop_debug` as a temporary runtime-only toggle.

## Goals
- Generate and load a CounterStrikeSharp plugin config file for RealBhop CS2.
- Allow server operators to change movement/configuration values without recompiling the plugin.
- Add `css_realbhop_reload` to reload config values during runtime.
- Keep `css_realbhop_debug` temporary and in-memory only; it must not update or persist config files.
- Preserve existing behavior when no config file exists by using the current defaults.

## Non-Goals
- Adding a custom config parser outside CounterStrikeSharp.
- Persisting runtime command changes back to disk.
- Adding per-player configuration.

## Architecture
`RealBhopConfig` will inherit from `BasePluginConfig`, allowing CounterStrikeSharp to generate and load the plugin config file. `RealBhopPlugin` will implement `IPluginConfig<RealBhopConfig>` and receive parsed config through `OnConfigParsed` before plugin load.

Runtime systems should continue to read the active `RealBhopConfig` instance. Because CounterStrikeSharp's `Reload()` updates the existing object in place, existing references held by commands, tracking, movement, and diagnostics remain valid.

## Runtime Debug Behavior
The `Debug` config property remains available as the file-backed startup/default value. The `css_realbhop_debug` command toggles an in-memory override only. The command must not call `Config.Update()` or write the config file.

Reload behavior:
- Reload reads file-backed config values again.
- The temporary debug override stays active until toggled again or plugin unload.
- Status output should make it clear what the effective debug state is.

## Commands
- `css_realbhop_status`: shows loaded runtime configuration and effective debug state.
- `css_realbhop_debug`: toggles temporary in-memory debug override only.
- `css_realbhop_reload`: reloads the CounterStrikeSharp config file and reports success.
- `css_realbhop_reset`: unchanged.

## Error Handling
If CounterStrikeSharp cannot load the config file, its standard config loading behavior applies during plugin initialization. The reload command should catch reload failures, report the error to the command caller, and keep the previous in-memory values active.

## Tests
Add or update unit-testable logic around runtime config state:
- Effective debug uses config `Debug` when no override exists.
- Debug toggle changes only runtime override.
- Config reload application can preserve the runtime debug override.
- Status formatting includes enough information to distinguish config debug from effective debug.

## Documentation
Update README configuration instructions to mention the CounterStrikeSharp config path and reload/debug commands.

## Success Criteria
- Fresh installs generate/use a CounterStrikeSharp config file with current defaults.
- Editing config and running `css_realbhop_reload` updates movement/runtime settings without recompiling.
- `css_realbhop_debug` changes do not persist to config.
- Existing tests pass and new config behavior tests pass.
