# RealBhop_CS2: The CSSHarp plugin reimplementation of RealBhop: HL1-like Bhopping [Github](https://github.com/SeriTools/sm_realbhop) [AlliedModders](https://forums.alliedmods.net/showthread.php?t=244387)

# Introduction
This plugins is an reimplementation of SeriTools's RealBhop plugins for Counter-Strike 2, aims to recreate HL1/Quake-like bunnyhopping.

# Getting Started
This plugin requires [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp) installed. If you haven't install CounterStrikeSharp, head to [here](https://docs.cssharp.dev/docs/guides/getting-started.html)

1. Download the latest [release](https://github.com/b4iterdev/RealBhop_CS2/releases) asset from the GitHub Releases page.
2. Copy the extracted plugin folder into:

   `addons/counterstrikesharp/plugins/RealBhopCS2`

3. Restart the server.
4. Verify it loaded with `css_realbhop_status` command.

To build from source instead of using releases:

```bash
dotnet publish src/RealBhopCS2/RealBhopCS2.csproj --configuration Release --output artifacts/RealBhopCS2
```

Copy `artifacts/RealBhopCS2` into the CounterStrikeSharp plugins directory.

# Configuration
Configuration lives in the plugin config file and defaults mirror sm_realbhop behavior. The important fields are:

- `Enabled` — master on/off switch.
- `MaxBhopTicks` — max ground ticks after landing where correction can still apply (sm_realbhop’s maxbhopframes).
- `FramePenalty` — late-jump penalty multiplier per ground tick (sm_realbhop’s framepenalty).
- `ApplyOnlyHorizontalCorrection` — apply correction to X/Y only (recommended).
- `MaxCorrectionSpeed` — safety cap for correction magnitude.
- `IgnoreBots` — skip bot clients.
- `Debug` — enable debug logs and commands.
- `SkipTriggerPush` — ignore trigger_push volumes (matches sm_realbhop behavior).
- `ExperimentalAirAccelerate` — optional HL1-style air accelerate model.
- `Hl1AirAccelerate` — air acceleration strength when experimental mode is enabled.
- `Hl1MaxSpeed` — max speed target for experimental HL1 air accelerate.

# How does it work ?
This plugin mirrors sm_realbhop’s HL1/Quake-style speed preservation. It records the last air-frame velocity before landing, then on the second air frame after a short ground touch it applies a correction toward that speed. Late jumps receive a penalty via `FramePenalty`, and after `MaxBhopTicks` no correction is applied.

What players feel: clean hops carry speed more consistently, late hops lose speed, and long ground contact breaks the chain.

# Credits
- Valve: Counter-Strike 2
- [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp)
- SeriTools: [sm_realbhop](https://github.com/SeriTools/sm_realbhop)
- [Sisyphus](https://github.com/code-yeongyu/oh-my-openagent)