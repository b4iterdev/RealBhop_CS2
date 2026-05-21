using CounterStrikeSharp.API.Core;
using RealBhopCS2.Config;
using RealBhopCS2.Tracking;

namespace RealBhopCS2;

public sealed class RealBhopPlugin : BasePlugin
{
    private RealBhopConfig _config = new();
    private BhopTracker _tracker = new(new RealBhopConfig());

    public override string ModuleName => "RealBhop CS2";

    public override string ModuleVersion => "0.1.0";

    public override string ModuleAuthor => "b4iterdev";

    public override string ModuleDescription => "CounterStrikeSharp prototype for sm_realbhop-style bunnyhop speed preservation.";

    public override void Load(bool hotReload)
    {
        _config = new RealBhopConfig();
        _tracker = new BhopTracker(_config);

        RegisterListener<Listeners.OnTick>(OnTick);
        RegisterListener<Listeners.OnClientDisconnect>(OnClientDisconnect);

        Console.WriteLine($"{ModuleName} {ModuleVersion} loaded. enabled={_config.Enabled} max_bhop_ticks={_config.MaxBhopTicks} frame_penalty={_config.FramePenalty}");
    }

    public override void Unload(bool hotReload)
    {
        RemoveListener<Listeners.OnTick>(OnTick);
        RemoveListener<Listeners.OnClientDisconnect>(OnClientDisconnect);
        Console.WriteLine($"{ModuleName} unloaded.");
    }

    private void OnTick()
    {
        // CS2 API binding is intentionally left thin in this first checkpoint.
        // The tested BhopTracker state machine owns movement decisions; a later
        // step wires player snapshots and velocity application to this tick.
        _ = _tracker;
    }

    private void OnClientDisconnect(int playerSlot)
    {
        // Per-player state cleanup will be added when player snapshot binding is wired.
        _ = playerSlot;
    }
}
