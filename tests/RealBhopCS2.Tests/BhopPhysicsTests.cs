using RealBhopCS2.Config;
using RealBhopCS2.Movement;

namespace RealBhopCS2.Tests;

public sealed class BhopPhysicsTests
{
    [Fact]
    public void CalculateCorrection_NoPenalty_RestoresHorizontalVelocityLoss()
    {
        var config = new RealBhopConfig
        {
            FramePenalty = 0.975f,
            MaxCorrectionSpeed = 3500.0f
        };

        var correction = BhopPhysics.CalculateCorrection(
            new MovementVector(300.0f, 0.0f, 0.0f),
            new MovementVector(250.0f, 0.0f, 0.0f),
            groundTicks: 0,
            config);

        Assert.Equal(50.0f, correction.X, precision: 2);
        Assert.Equal(0.0f, correction.Y, precision: 2);
        Assert.Equal(0.0f, correction.Z, precision: 2);
    }

    [Fact]
    public void CalculateCorrection_WithGroundTickPenalty_ReducesCorrection()
    {
        var config = new RealBhopConfig
        {
            FramePenalty = 0.975f,
            MaxCorrectionSpeed = 3500.0f
        };

        var correction = BhopPhysics.CalculateCorrection(
            new MovementVector(300.0f, 0.0f, 0.0f),
            new MovementVector(250.0f, 0.0f, 0.0f),
            groundTicks: 1,
            config);

        Assert.Equal(48.75f, correction.X, precision: 2);
        Assert.Equal(0.0f, correction.Y, precision: 2);
        Assert.Equal(0.0f, correction.Z, precision: 2);
    }

    [Fact]
    public void CalculateCorrection_WhenCorrectionExceedsMax_ClampsHorizontalLength()
    {
        var config = new RealBhopConfig
        {
            FramePenalty = 1.0f,
            MaxCorrectionSpeed = 100.0f
        };

        var correction = BhopPhysics.CalculateCorrection(
            new MovementVector(300.0f, 400.0f, 0.0f),
            new MovementVector(0.0f, 0.0f, 0.0f),
            groundTicks: 0,
            config);

        Assert.Equal(100.0f, correction.HorizontalLength, precision: 2);
        Assert.Equal(60.0f, correction.X, precision: 2);
        Assert.Equal(80.0f, correction.Y, precision: 2);
        Assert.Equal(0.0f, correction.Z, precision: 2);
    }

    [Fact]
    public void CalculateCorrection_WhenAirVelocityHasVerticalComponent_DoesNotApplyVerticalCorrection()
    {
        var config = new RealBhopConfig
        {
            FramePenalty = 1.0f,
            MaxCorrectionSpeed = 3500.0f
        };

        var correction = BhopPhysics.CalculateCorrection(
            new MovementVector(300.0f, 0.0f, 500.0f),
            new MovementVector(250.0f, 0.0f, -100.0f),
            groundTicks: 0,
            config);

        Assert.Equal(50.0f, correction.X, precision: 2);
        Assert.Equal(0.0f, correction.Y, precision: 2);
        Assert.Equal(0.0f, correction.Z, precision: 2);
    }
}
