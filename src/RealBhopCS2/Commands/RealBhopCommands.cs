using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using RealBhopCS2.Config;
using RealBhopCS2.Diagnostics;
using RealBhopCS2.Tracking;

namespace RealBhopCS2.Commands;

public sealed class RealBhopCommands
{
    private readonly RealBhopRuntimeConfig _config;
    private readonly PlayerStateRepository _states;
    private readonly Action _reloadConfig;

    public RealBhopCommands(RealBhopRuntimeConfig config, PlayerStateRepository states, Action reloadConfig)
    {
        _config = config;
        _states = states;
        _reloadConfig = reloadConfig;
    }

    public void Execute(CCSPlayerController? player, CommandInfo command)
    {
        switch (RealBhopCommandRouter.Parse(command.GetArg(1)))
        {
            case RealBhopCommandAction.Status:
                Status(player, command);
                break;
            case RealBhopCommandAction.Debug:
                ToggleDebug(player, command);
                break;
            case RealBhopCommandAction.Reload:
                Reload(player, command);
                break;
            case RealBhopCommandAction.Reset:
                Reset(player, command);
                break;
            case RealBhopCommandAction.Help:
            default:
                Help(command);
                break;
        }
    }

    public void Status(CCSPlayerController? player, CommandInfo command)
    {
        command.ReplyToCommand(RealBhopDebugFormatter.FormatStatus(_config, _states.Count));
    }

    public void ToggleDebug(CCSPlayerController? player, CommandInfo command)
    {
        if (!RealBhopCommandAccess.CanRunServerMutation(player))
        {
            command.ReplyToCommand("RealBhop debug can only be toggled from the server console.");
            return;
        }

        command.ReplyToCommand($"RealBhop debug={_config.ToggleDebug()} (temporary)");
    }

    public void Reload(CCSPlayerController? player, CommandInfo command)
    {
        if (!RealBhopCommandAccess.CanRunServerMutation(player))
        {
            command.ReplyToCommand("RealBhop config can only be reloaded from the server console.");
            return;
        }

        try
        {
            _reloadConfig();
            command.ReplyToCommand("RealBhop config reloaded.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"RealBhop config reload failed: {ex}");
            command.ReplyToCommand("RealBhop config reload failed. Check server logs for details.");
        }
    }

    public void Reset(CCSPlayerController? player, CommandInfo command)
    {
        if (!RealBhopCommandAccess.CanRunServerMutation(player))
        {
            command.ReplyToCommand("RealBhop state can only be reset from the server console.");
            return;
        }

        _states.ResetAll();
        command.ReplyToCommand("RealBhop state reset.");
    }

    private static void Help(CommandInfo command)
    {
        command.ReplyToCommand("Usage: css_realbhop <status|debug|reload|reset>");
    }
}
