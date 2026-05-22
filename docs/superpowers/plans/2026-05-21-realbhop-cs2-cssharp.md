# RealBhop CS2 CounterStrikeSharp Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use `superpowers:subagent-driven-development` or `superpowers:executing-plans` to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build a CounterStrikeSharp plugin that recreates `sm_realbhop`-style bunnyhop speed preservation in CS2, with optional HL1-inspired tuning.

**Architecture:** Start with a safe CounterStrikeSharp-only version that observes player movement each tick and applies velocity correction after valid jump timing. Keep the system modular so later native/Metamod hooks can replace timing-sensitive logic if needed.

**Tech Stack:** CounterStrikeSharp, .NET 8, C#, CS2 dedicated server, Metamod.

---

## 1. Scope

### Version 1 goal

Implement a CS2 equivalent of `sm_realbhop`:

- Detect player ground/air transitions.
- Store last valid air velocity.
- Track how many ticks player stayed grounded before jumping.
- If jump occurs within configurable frame window, restore lost horizontal velocity.
- Apply configurable late-jump penalty.
- Skip logic in invalid movement states.
- Provide config and debug tools.

### Not in Version 1

Do **not** implement full HL1 movement yet:

- no full custom `PM_AirAccelerate`
- no edgefriction
- no subtick usercmd normalization
- no native movement detours
- no replacement physics engine

Those belong to Version 2 / native layer.

---

## 2. Proposed file structure

```text
RealBhopCS2/
  RealBhopCS2.csproj
  RealBhopPlugin.cs
  Config/
    RealBhopConfig.cs
  Movement/
    PlayerBhopState.cs
    MovementSnapshot.cs
    BhopPhysics.cs
    PlayerMovementReader.cs
    PlayerVelocityApplier.cs
  Tracking/
    BhopTracker.cs
    TriggerPushTracker.cs
  Commands/
    RealBhopCommands.cs
  Diagnostics/
    DebugOverlay.cs
  Tests/
    RealBhopCS2.Tests.csproj
    BhopPhysicsTests.cs
```

### Responsibility map

| File | Responsibility |
|---|---|
| `RealBhopPlugin.cs` | Plugin entry point, load/unload, event registration |
| `RealBhopConfig.cs` | Config values for bhop behavior |
| `PlayerBhopState.cs` | Per-player stored state |
| `MovementSnapshot.cs` | Immutable movement data for a player on one tick |
| `BhopPhysics.cs` | Velocity correction math |
| `PlayerMovementReader.cs` | Reads CS2 pawn/controller movement state |
| `PlayerVelocityApplier.cs` | Applies corrected velocity |
| `BhopTracker.cs` | Main per-tick state machine |
| `TriggerPushTracker.cs` | Detects/skips trigger push zones if possible |
| `RealBhopCommands.cs` | Admin/debug commands |
| `DebugOverlay.cs` | Optional debug logging/chat output |

---

## 3. Configuration design

```csharp
public sealed class RealBhopConfig
{
    public bool Enabled { get; set; } = true;
    public int MaxBhopTicks { get; set; } = 12;
    public float FramePenalty { get; set; } = 0.975f;
    public bool ApplyOnlyHorizontalCorrection { get; set; } = true;
    public float MaxCorrectionSpeed { get; set; } = 3500.0f;
    public bool IgnoreBots { get; set; } = true;
    public bool Debug { get; set; } = false;
    public bool SkipTriggerPush { get; set; } = true;
    public bool ExperimentalAirAccelerate { get; set; } = false;
    public float Hl1AirAccelerate { get; set; } = 10.0f;
    public float Hl1MaxSpeed { get; set; } = 320.0f;
}
```

---

## 4. Implementation tasks

### Task 1: Create base plugin skeleton

**Files:**

- Create: `RealBhopCS2.csproj`
- Create: `RealBhopPlugin.cs`

- [ ] Create `.csproj` targeting .NET 8.
- [ ] Reference CounterStrikeSharp API.
- [ ] Create plugin class inheriting `BasePlugin`.
- [ ] Add metadata: module name, author, version, description.
- [ ] Register `OnTick`.
- [ ] Register player disconnect cleanup.
- [ ] Add basic load/unload logging.

Expected behavior: server starts, plugin loads, console prints `RealBhopCS2 loaded`, no movement logic yet.

### Task 2: Add config model

**Files:**

- Create: `Config/RealBhopConfig.cs`
- Modify: `RealBhopPlugin.cs`

- [ ] Define `RealBhopConfig`.
- [ ] Load config during plugin startup.
- [ ] Add defaults matching `sm_realbhop`: enabled true, max bhop ticks 12, frame penalty 0.975.
- [ ] Add CS2-specific safety limits: max correction speed, ignore bots, debug toggle.
- [ ] Log loaded config.

### Task 3: Define per-player movement state

**Files:**

- Create: `Movement/PlayerBhopState.cs`
- Create: `Movement/MovementSnapshot.cs`

- [ ] Store `WasOnGround`, `AfterJumpFrame`, `GroundTicks`, `LastAirVelocity`, `LastGroundVelocity`, `InTriggerPush`, `LastButtons`, `LastProcessedTick`.
- [ ] Reset state on disconnect.
- [ ] Reset state on map start.

### Task 4: Implement movement reader

**Files:**

- Create: `Movement/PlayerMovementReader.cs`

- [ ] Check player/controller validity.
- [ ] Get pawn safely.
- [ ] Check alive state.
- [ ] Read velocity.
- [ ] Read button state.
- [ ] Read ground state.
- [ ] Skip spectator, no pawn, dead, bot if ignored, noclip, ladder, water when detectable.

### Task 5: Implement bhop correction math using TDD

**Files:**

- Create: `Movement/BhopPhysics.cs`
- Create: `Tests/BhopPhysicsTests.cs`

- [ ] Write failing test: no-penalty horizontal correction restores velocity loss.
- [ ] Run test and verify it fails because `BhopPhysics` is missing.
- [ ] Implement minimal `CalculateCorrection`.
- [ ] Run test and verify pass.
- [ ] Write failing test: frame penalty reduces correction.
- [ ] Implement penalty support.
- [ ] Write failing test: vertical correction is never applied.
- [ ] Implement/verify Z clamp.
- [ ] Write failing test: max correction clamps excessive values.
- [ ] Implement/verify correction clamp.

### Task 6: Implement velocity applier

**Files:**

- Create: `Movement/PlayerVelocityApplier.cs`

- [ ] Start with `Teleport(null, null, newVelocity)` velocity application.
- [ ] Preserve current Z velocity.
- [ ] Add guard against non-finite correction vectors.

### Task 7: Implement main bhop state machine

**Files:**

- Create: `Tracking/BhopTracker.cs`
- Modify: `RealBhopPlugin.cs`

- [ ] Wire `OnTick` to `BhopTracker.Tick`.
- [ ] Track first ground frame.
- [ ] Increment ground ticks while grounded.
- [ ] Store last air velocity every airborne tick after first air frame.
- [ ] Apply correction once on second air frame if `GroundTicks <= MaxBhopTicks`.
- [ ] Reset invalid players.
- [ ] Skip trigger push states.

### Task 8: Add admin/debug commands

**Files:**

- Create: `Commands/RealBhopCommands.cs`

- [ ] Add `css_realbhop status`.
- [ ] Add `css_realbhop debug`.
- [ ] Add `css_realbhop reload`.
- [ ] Add `css_realbhop reset`.

### Task 9: Add debug diagnostics and running notes

**Files:**

- Create: `Diagnostics/DebugOverlay.cs`
- Modify: `Tracking/BhopTracker.cs`
- Maintain: `docs/implementation-notes/realbhop-cs2-execution-notes.md`

- [ ] Print per-jump debug values when enabled.
- [ ] Record obstacles encountered during implementation.
- [ ] Record trade-offs made during implementation.
- [ ] Notify user if a blocker needs attention.

### Task 10: Add trigger/push skipping

**Files:**

- Create: `Tracking/TriggerPushTracker.cs`
- Modify: `Tracking/BhopTracker.cs`

- [ ] If clean trigger touch hooks are available, track `trigger_push` enter/exit.
- [ ] Otherwise implement conservative external impulse skip threshold.
- [ ] Document the trade-off.

### Task 11: Manual server testing checklist

- [ ] Standing still jump: no horizontal boost.
- [ ] Normal running jump: no weird vertical change.
- [ ] Perfect repeated bhop: speed preservation improved.
- [ ] Late jump: speed preservation reduced.
- [ ] Very late jump: no correction.
- [ ] Trigger push: correction skipped.
- [ ] Ladder/water/noclip: correction skipped when detectable.
- [ ] Bot ignored.
- [ ] Plugin reload applies config.
- [ ] Map change resets states.

---

## 5. Version 2: HL1-inspired movement layer

After Version 1 works, add optional HL1 behavior:

- HL1 air acceleration approximation.
- `wishdir` / `wishspeed` calculation from movement input and view angles.
- 30-unit air wishspeed cap behavior.
- Optional Steam-era bhop cap.
- Native hook research spike for `ProcessUsercmds`, `ProcessMovement`, `CheckMovingGround`, `WalkMove`, `AirMove`, `Jump`, and `Friction`.

---

## 6. Completion criteria

Version 1 is complete when:

- Plugin loads on CS2 server.
- No errors on map change/reload/disconnect.
- Basic bhop speed preservation works.
- Late jumps are penalized.
- Debug output confirms correction math.
- Trigger/external impulse behavior does not break maps.
- Unit tests for correction math pass.
- Manual server testing confirms no vertical boost abuse.

Final recommendation: build Version 1 in CounterStrikeSharp first. Only move to native hooks after measuring exactly where CSSharp timing fails.
