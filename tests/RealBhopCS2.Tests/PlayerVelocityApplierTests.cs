using RealBhopCS2.Movement;

namespace RealBhopCS2.Tests;

public sealed class PlayerVelocityApplierTests
{
    [Fact]
    public void BuildBaseVelocityCorrection_ReturnsCorrectionWithoutAddingCurrentVelocity()
    {
        var currentVelocity = new MovementVector(250.0f, 25.0f, 5.0f);
        var correction = new MovementVector(50.0f, -10.0f, 0.0f);

        var baseVelocity = PlayerVelocityApplier.BuildBaseVelocityCorrection(currentVelocity, correction);

        Assert.Equal(50.0f, baseVelocity.X, precision: 2);
        Assert.Equal(-10.0f, baseVelocity.Y, precision: 2);
        Assert.Equal(0.0f, baseVelocity.Z, precision: 2);
    }
}
