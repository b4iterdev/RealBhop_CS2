using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using RealBhopCS2.Config;
using RealBhopCS2.Diagnostics;
using RealBhopCS2.Tracking;

namespace RealBhopCS2.Commands;

public sealed class RealBhopCommands
{
    private readonly RealBhopConfig _config;
    private readonly PlayerStateRepository _states;

    public RealBhopCommands(RealBhopConfig config, PlayerStateRepository states)
    {
        _config = config;
        _states = states;
    }

    public void Status(CCSPlayerController? player, CommandInfo command)
    {
        command.ReplyToCommand(RealBhopDebugFormatter.FormatStatus(_config, _states.Count));
    }

    public void ToggleDebug(CCSPlayerController? player, CommandInfo command)
    {
        _config.Debug = !_config.Debug;
        command.ReplyToCommand($"RealBhop debug={_config.Debug}");
    }

    public void Reset(CCSPlayerController? player, CommandInfo command)
    {
        _states.ResetAll();
        command.ReplyToCommand("RealBhop state reset.");
    }
}
