using RealBhopCS2;

namespace RealBhopCS2.Tests;

public sealed class RealBhopLoopTimingTests
{
    [Fact]
    public void CorrectionLoopListenerName_UsesServerPreEntityThink()
    {
        Assert.Equal("OnServerPreEntityThink", RealBhopLoopTiming.CorrectionLoopListenerName);
    }
}
