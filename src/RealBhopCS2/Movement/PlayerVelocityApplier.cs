using CounterStrikeSharp.API.Core;

namespace RealBhopCS2.Movement;

public sealed class PlayerVelocityApplier
{
    public bool TryApply(CCSPlayerController controller, MovementSnapshot snapshot, MovementVector correction)
    {
        if (!controller.IsValid || !IsFinite(correction))
        {
            return false;
        }

        var pawn = controller.PlayerPawn.Value;
        if (pawn is null || !pawn.IsValid)
        {
            return false;
        }

        var velocity = new System.Numerics.Vector3(
            snapshot.Velocity.X + correction.X,
            snapshot.Velocity.Y + correction.Y,
            snapshot.Velocity.Z);

        pawn.Teleport(null, null, velocity);
        return true;
    }

    private static bool IsFinite(MovementVector vector)
    {
        return float.IsFinite(vector.X) && float.IsFinite(vector.Y) && float.IsFinite(vector.Z);
    }
}
