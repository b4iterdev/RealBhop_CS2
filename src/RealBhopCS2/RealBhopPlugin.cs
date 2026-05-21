using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Extensions;
using RealBhopCS2.Commands;
using RealBhopCS2.Config;
using RealBhopCS2.Diagnostics;
using RealBhopCS2.Movement;
using RealBhopCS2.Tracking;

namespace RealBhopCS2;

public sealed class RealBhopPlugin : BasePlugin, IPluginConfig<RealBhopConfig>
{
    private RealBhopRuntimeConfig _config = new(new RealBhopConfig());
    private BhopTracker _tracker = new(new RealBhopConfig());
    private PlayerStateRepository _states = new();
    private PlayerMovementReader _movementReader = new();
    private PlayerVelocityApplier _velocityApplier = new();
    private RealBhopCommands? _commands;

    public override string ModuleName => "RealBhop CS2";

    public override string ModuleVersion => "0.2.0";

    public override string ModuleAuthor => "b4iterdev";

    public override string ModuleDescription => "CounterStrikeSharp prototype for sm_realbhop-style bunnyhop speed preservation.";

    public RealBhopConfig Config { get; set; } = new();

    public void OnConfigParsed(RealBhopConfig config)
    {
        Config = config;
        _config.ReplaceConfig(config);
    }

    public override void Load(bool hotReload)
    {
        _config.ReplaceConfig(Config);
        _tracker = new BhopTracker(_config.Config);
        _states = new PlayerStateRepository();
        _movementReader = new PlayerMovementReader();
        _velocityApplier = new PlayerVelocityApplier();
        _commands = new RealBhopCommands(_config, _states, ReloadConfig);

        RegisterListener<Listeners.OnTick>(OnTick);
        RegisterListener<Listeners.OnClientDisconnect>(OnClientDisconnect);

        AddCommand("css_realbhop_status", "Print RealBhop runtime status.", _commands.Status);
        AddCommand("css_realbhop_debug", "Toggle RealBhop debug output.", _commands.ToggleDebug);
        AddCommand("css_realbhop_reload", "Reload RealBhop config from disk.", _commands.Reload);
        AddCommand("css_realbhop_reset", "Reset RealBhop per-player movement state.", _commands.Reset);

        Console.WriteLine($"{ModuleName} {ModuleVersion} loaded. enabled={_config.Config.Enabled} max_bhop_ticks={_config.Config.MaxBhopTicks} frame_penalty={_config.Config.FramePenalty}");
    }

    public override void Unload(bool hotReload)
    {
        RemoveListener<Listeners.OnTick>(OnTick);
        RemoveListener<Listeners.OnClientDisconnect>(OnClientDisconnect);

        if (_commands is not null)
        {
            RemoveCommand("css_realbhop_status", _commands.Status);
            RemoveCommand("css_realbhop_debug", _commands.ToggleDebug);
            RemoveCommand("css_realbhop_reload", _commands.Reload);
            RemoveCommand("css_realbhop_reset", _commands.Reset);
        }

        Console.WriteLine($"{ModuleName} unloaded.");
    }

    private void OnTick()
    {
        foreach (var player in _movementReader.GetPlayers())
        {
            var state = _states.GetOrCreate(player.Slot);
            var snapshot = _movementReader.TryRead(player, Server.TickCount, _config.Config, state.InTriggerPush);
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

            if (_config.EffectiveDebug && ShouldLogDebugResult(result))
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

    private void ReloadConfig()
    {
        Config.Reload();
        _config.ReplaceConfig(Config);
    }
}
