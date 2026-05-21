using RealBhopCS2.Movement;
using RealBhopCS2.Tracking;

namespace RealBhopCS2.Tests;

public sealed class PlayerStateRepositoryTests
{
    [Fact]
    public void GetOrCreate_ReturnsSameStateForSameSlot()
    {
        var repository = new PlayerStateRepository();

        var first = repository.GetOrCreate(7);
        first.GroundTicks = 3;

        var second = repository.GetOrCreate(7);

        Assert.Same(first, second);
        Assert.Equal(3, second.GroundTicks);
        Assert.Equal(1, repository.Count);
    }

    [Fact]
    public void Remove_DeletesExistingState()
    {
        var repository = new PlayerStateRepository();
        repository.GetOrCreate(12).LastAirVelocity = new MovementVector(100.0f, 0.0f, 0.0f);

        var removed = repository.Remove(12);

        Assert.True(removed);
        Assert.Equal(0, repository.Count);
    }

    [Fact]
    public void ResetAll_ClearsAllTrackedStates()
    {
        var repository = new PlayerStateRepository();
        repository.GetOrCreate(1);
        repository.GetOrCreate(2);

        repository.ResetAll();

        Assert.Equal(0, repository.Count);
    }
}
