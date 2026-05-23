using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace RealBhopCS2.Movement;

public sealed class PlayerVelocityApplier
{
    public static Vector BuildBaseVelocityCorrection(MovementVector currentVelocity, MovementVector correction)
    {
        return new Vector(correction.X, correction.Y, correction.Z);
    }

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

        var baseVelocity = BuildBaseVelocityCorrection(snapshot.Velocity, correction);

        pawn.BaseVelocity.X = baseVelocity.X;
        pawn.BaseVelocity.Y = baseVelocity.Y;
        pawn.BaseVelocity.Z = baseVelocity.Z;
        return true;
    }

    private static bool IsFinite(MovementVector vector)
    {
        return float.IsFinite(vector.X) && float.IsFinite(vector.Y) && float.IsFinite(vector.Z);
    }
}
