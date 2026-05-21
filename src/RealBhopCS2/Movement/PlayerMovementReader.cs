using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using RealBhopCS2.Config;

namespace RealBhopCS2.Movement;

public sealed class PlayerMovementReader
{
    public IReadOnlyList<CCSPlayerController> GetPlayers()
    {
        return Utilities.GetPlayers();
    }

    public MovementSnapshot? TryRead(CCSPlayerController controller, int tick, RealBhopConfig config, bool isInTriggerPush)
    {
        if (!controller.IsValid)
        {
            return null;
        }

        if (config.IgnoreBots && controller.IsBot)
        {
            return null;
        }

        var pawn = controller.PlayerPawn.Value;
        if (pawn is null || !pawn.IsValid)
        {
            return null;
        }

        var velocity = pawn.AbsVelocity;
        var isOnGround = pawn.GroundEntity.IsValid;
        var isValidForBhop = controller.PawnIsAlive;

        return new MovementSnapshot(
            tick,
            isOnGround,
            isValidForBhop,
            isInTriggerPush,
            new MovementVector(velocity.X, velocity.Y, velocity.Z));
    }
}
