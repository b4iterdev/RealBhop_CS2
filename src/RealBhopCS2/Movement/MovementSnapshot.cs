namespace RealBhopCS2.Movement;

public sealed record MovementSnapshot(
    int Tick,
    bool IsOnGround,
    bool IsValidForBhop,
    bool IsInTriggerPush,
    MovementVector Velocity);
