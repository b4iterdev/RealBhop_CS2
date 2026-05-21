using RealBhopCS2.Config;
using RealBhopCS2.Movement;

namespace RealBhopCS2.Tracking;

public sealed class BhopTracker
{
    private readonly RealBhopConfig _config;

    public BhopTracker(RealBhopConfig config)
    {
        _config = config;
    }

    public BhopTickResult Process(MovementSnapshot snapshot, PlayerBhopState state)
    {
        state.LastProcessedTick = snapshot.Tick;

        if (!_config.Enabled)
        {
            return BhopTickResult.None("Disabled");
        }

        if (!snapshot.IsValidForBhop)
        {
            state.Reset();
            return BhopTickResult.None("InvalidMovementState");
        }

        if (snapshot.IsInTriggerPush || state.InTriggerPush)
        {
            return BhopTickResult.None("TriggerPush");
        }

        if (snapshot.IsOnGround)
        {
            if (!state.WasOnGround)
            {
                state.WasOnGround = true;
                state.GroundTicks = 0;
            }
            else
            {
                state.GroundTicks++;
            }

            state.LastGroundVelocity = snapshot.Velocity;
            return BhopTickResult.None("Grounded");
        }

        if (state.AfterJumpFrame)
        {
            state.AfterJumpFrame = false;

            if (state.GroundTicks <= _config.MaxBhopTicks)
            {
                var correction = BhopPhysics.CalculateCorrection(
                    state.LastAirVelocity,
                    snapshot.Velocity,
                    state.GroundTicks,
                    _config);

                return correction.HorizontalLength > 0.0f
                    ? BhopTickResult.Apply(correction)
                    : BhopTickResult.None("ZeroCorrection");
            }

            return BhopTickResult.None("GroundTicksExceeded");
        }

        if (state.WasOnGround)
        {
            state.WasOnGround = false;
            state.AfterJumpFrame = true;
            return BhopTickResult.None("FirstAirFrame");
        }

        state.LastAirVelocity = snapshot.Velocity;
        return BhopTickResult.None("Airborne");
    }
}
