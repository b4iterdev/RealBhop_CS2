using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using RealBhopCS2.Commands;
using RealBhopCS2.Config;
using RealBhopCS2.Diagnostics;
using RealBhopCS2.Movement;
using RealBhopCS2.Tracking;

namespace RealBhopCS2;

public sealed class RealBhopPlugin : BasePlugin
{
    private RealBhopConfig _config = new();
    private BhopTracker _tracker = new(new RealBhopConfig());
    private PlayerStateRepository _states = new();
    private PlayerMovementReader _movementReader = new();
    private PlayerVelocityApplier _velocityApplier = new();
    private RealBhopCommands _commands = null!;

    public override string ModuleName => "RealBhop CS2";

    public override string ModuleVersion => "0.2.0";

    public override string ModuleAuthor => "b4iterdev";

    public override string ModuleDescription => "CounterStrikeSharp prototype for sm_realbhop-style bunnyhop speed preservation.";

    public override void Load(bool hotReload)
    {
        _config = new RealBhopConfig();
        _tracker = new BhopTracker(_config);
        _states = new PlayerStateRepository();
        _movementReader = new PlayerMovementReader();
        _velocityApplier = new PlayerVelocityApplier();
        _commands = new RealBhopCommands(_config, _states);

        RegisterListener<Listeners.OnTick>(OnTick);
        RegisterListener<Listeners.OnClientDisconnect>(OnClientDisconnect);

        AddCommand("css_realbhop_status", "Print RealBhop runtime status.", _commands.Status);
        AddCommand("css_realbhop_debug", "Toggle RealBhop debug output.", _commands.ToggleDebug);
        AddCommand("css_realbhop_reset", "Reset RealBhop per-player movement state.", _commands.Reset);

        Console.WriteLine($"{ModuleName} {ModuleVersion} loaded. enabled={_config.Enabled} max_bhop_ticks={_config.MaxBhopTicks} frame_penalty={_config.FramePenalty}");
    }

    public override void Unload(bool hotReload)
    {
        RemoveListener<Listeners.OnTick>(OnTick);
        RemoveListener<Listeners.OnClientDisconnect>(OnClientDisconnect);

        RemoveCommand("css_realbhop_status", _commands.Status);
        RemoveCommand("css_realbhop_debug", _commands.ToggleDebug);
        RemoveCommand("css_realbhop_reset", _commands.Reset);

        Console.WriteLine($"{ModuleName} unloaded.");
    }

    private void OnTick()
    {
        foreach (var player in _movementReader.GetPlayers())
        {
            var state = _states.GetOrCreate(player.Slot);
            var snapshot = _movementReader.TryRead(player, Server.TickCount, _config, state.InTriggerPush);
            if (snapshot is null)
            {
                state.Reset();
                continue;
            }

            var result = _tracker.Process(snapshot, state);
            if (result.ShouldApplyCorrection)
            {
                _velocityApplier.TryApply(player, snapshot, result.Correction);
            }

            if (_config.Debug && ShouldLogDebugResult(result))
            {
                Console.WriteLine(RealBhopDebugFormatter.FormatCorrection(player.PlayerName, snapshot, state, result));
            }
        }
    }

    private static bool ShouldLogDebugResult(BhopTickResult result)
    {
        return result.ShouldApplyCorrection
               || result.Reason is "NoPreviousAirVelocity" or "GroundTicksExceeded" or "ZeroCorrection" or "TriggerPush";
    }

    private void OnClientDisconnect(int playerSlot)
    {
        _states.Remove(playerSlot);
    }
}
