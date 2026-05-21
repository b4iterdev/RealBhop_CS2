using RealBhopCS2.Config;

namespace RealBhopCS2.Tests;

public sealed class RealBhopRuntimeConfigTests
{
    [Fact]
    public void EffectiveDebug_UsesConfigDebug_WhenNoTemporaryOverrideExists()
    {
        var runtimeConfig = new RealBhopRuntimeConfig(new RealBhopConfig { Debug = true });

        Assert.True(runtimeConfig.EffectiveDebug);
    }

    [Fact]
    public void ToggleDebug_ChangesEffectiveDebugWithoutMutatingConfigDebug()
    {
        var config = new RealBhopConfig { Debug = false };
        var runtimeConfig = new RealBhopRuntimeConfig(config);

        var effectiveDebug = runtimeConfig.ToggleDebug();

        Assert.True(effectiveDebug);
        Assert.False(config.Debug);
    }

    [Fact]
    public void ReplaceConfig_PreservesTemporaryDebugOverride()
    {
        var runtimeConfig = new RealBhopRuntimeConfig(new RealBhopConfig { Debug = false });
        runtimeConfig.ToggleDebug();

        runtimeConfig.ReplaceConfig(new RealBhopConfig { Debug = false });

        Assert.True(runtimeConfig.EffectiveDebug);
        Assert.False(runtimeConfig.Config.Debug);
    }
}
