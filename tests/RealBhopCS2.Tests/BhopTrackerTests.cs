using RealBhopCS2.Config;
using RealBhopCS2.Movement;
using RealBhopCS2.Tracking;

namespace RealBhopCS2.Tests;

public sealed class BhopTrackerTests
{
    [Fact]
    public void Process_WhenPlayerLeavesGroundWithinWindow_AppliesCorrectionOnce()
    {
        var tracker = new BhopTracker(new RealBhopConfig
        {
            MaxBhopTicks = 12,
            FramePenalty = 1.0f,
            MaxCorrectionSpeed = 3500.0f
        });
        var state = new PlayerBhopState
        {
            WasOnGround = false,
            LastAirVelocity = new MovementVector(300.0f, 0.0f, 0.0f),
            HasLastAirVelocity = true
        };

        tracker.Process(new MovementSnapshot(1, IsOnGround: true, IsValidForBhop: true, IsInTriggerPush: false, Velocity: new MovementVector(250.0f, 0.0f, 0.0f)), state);
        var firstAirFrame = tracker.Process(new MovementSnapshot(2, IsOnGround: false, IsValidForBhop: true, IsInTriggerPush: false, Velocity: new MovementVector(250.0f, 0.0f, 0.0f)), state);
        var secondAirFrame = tracker.Process(new MovementSnapshot(3, IsOnGround: false, IsValidForBhop: true, IsInTriggerPush: false, Velocity: new MovementVector(250.0f, 0.0f, 0.0f)), state);
        var thirdAirFrame = tracker.Process(new MovementSnapshot(4, IsOnGround: false, IsValidForBhop: true, IsInTriggerPush: false, Velocity: new MovementVector(250.0f, 0.0f, 0.0f)), state);

        Assert.False(firstAirFrame.ShouldApplyCorrection);
        Assert.True(secondAirFrame.ShouldApplyCorrection);
        Assert.Equal(50.0f, secondAirFrame.Correction.X, precision: 2);
        Assert.False(thirdAirFrame.ShouldApplyCorrection);
    }

    [Fact]
    public void Process_WhenNoPreviousAirVelocityExists_DoesNotApplyStartupCorrection()
    {
        var tracker = new BhopTracker(new RealBhopConfig
        {
            MaxBhopTicks = 12,
            FramePenalty = 1.0f,
            MaxCorrectionSpeed = 3500.0f
        });
        var state = new PlayerBhopState();

        tracker.Process(new MovementSnapshot(1, IsOnGround: true, IsValidForBhop: true, IsInTriggerPush: false, Velocity: new MovementVector(250.0f, 0.0f, 0.0f)), state);
        tracker.Process(new MovementSnapshot(2, IsOnGround: false, IsValidForBhop: true, IsInTriggerPush: false, Velocity: new MovementVector(250.0f, 0.0f, 0.0f)), state);
        var secondAirFrame = tracker.Process(new MovementSnapshot(3, IsOnGround: false, IsValidForBhop: true, IsInTriggerPush: false, Velocity: new MovementVector(250.0f, 0.0f, 0.0f)), state);

        Assert.False(secondAirFrame.ShouldApplyCorrection);
        Assert.Equal("NoPreviousAirVelocity", secondAirFrame.Reason);
    }
}
