using CounterStrikeSharp.API.Core;

namespace RealBhopCS2.Commands;

public static class RealBhopCommandAccess
{
    public static bool CanRunServerMutation(CCSPlayerController? player)
    {
        return player is null;
    }
}
