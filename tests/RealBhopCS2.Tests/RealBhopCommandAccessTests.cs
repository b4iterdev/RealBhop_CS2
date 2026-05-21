using RealBhopCS2.Commands;

namespace RealBhopCS2.Tests;

public sealed class RealBhopCommandAccessTests
{
    [Fact]
    public void CanRunServerMutation_AllowsServerConsole()
    {
        Assert.True(RealBhopCommandAccess.CanRunServerMutation(player: null));
    }
}
