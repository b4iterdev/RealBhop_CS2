namespace RealBhopCS2.Movement;

public sealed class PlayerBhopState
{
    public bool WasOnGround { get; set; }

    public bool AfterJumpFrame { get; set; }

    public int GroundTicks { get; set; }

    public MovementVector LastAirVelocity { get; set; }

    public MovementVector LastGroundVelocity { get; set; }

    public bool InTriggerPush { get; set; }

    public ulong LastButtons { get; set; }

    public int LastProcessedTick { get; set; }

    public void Reset()
    {
        WasOnGround = false;
        AfterJumpFrame = false;
        GroundTicks = 0;
        LastAirVelocity = default;
        LastGroundVelocity = default;
        InTriggerPush = false;
        LastButtons = 0;
        LastProcessedTick = 0;
    }
}
