using RealBhopCS2.Config;

namespace RealBhopCS2.Movement;

public static class BhopPhysics
{
    public static MovementVector CalculateCorrection(
        MovementVector lastAirVelocity,
        MovementVector currentVelocity,
        int groundTicks,
        RealBhopConfig config)
    {
        var penalty = MathF.Pow(config.FramePenalty, groundTicks);
        var correction = new MovementVector(
            (lastAirVelocity.X - currentVelocity.X) * penalty,
            (lastAirVelocity.Y - currentVelocity.Y) * penalty,
            0.0f);

        var horizontalLength = correction.HorizontalLength;
        if (horizontalLength <= config.MaxCorrectionSpeed || horizontalLength <= 0.0f)
        {
            return correction;
        }

        var scale = config.MaxCorrectionSpeed / horizontalLength;
        return new MovementVector(
            correction.X * scale,
            correction.Y * scale,
            0.0f);
    }
}
