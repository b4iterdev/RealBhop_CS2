using RealBhopCS2.Config;
using RealBhopCS2.Diagnostics;

namespace RealBhopCS2.Tests;

public sealed class RealBhopDebugFormatterTests
{
    [Fact]
    public void FormatStatus_IncludesRuntimeConfigurationAndTrackedPlayerCount()
    {
        var config = new RealBhopConfig
        {
            Enabled = true,
            MaxBhopTicks = 12,
            FramePenalty = 0.975f,
            Debug = false
        };

        var runtimeConfig = new RealBhopRuntimeConfig(config);
        runtimeConfig.ToggleDebug();

        var status = RealBhopDebugFormatter.FormatStatus(runtimeConfig, trackedPlayers: 3);

        Assert.Contains("RealBhop: enabled=True", status);
        Assert.Contains("MaxBhopTicks=12", status);
        Assert.Contains("FramePenalty=0.975", status);
        Assert.Contains("Debug=True", status);
        Assert.Contains("ConfigDebug=False", status);
        Assert.Contains("PlayersTracked=3", status);
    }
}
