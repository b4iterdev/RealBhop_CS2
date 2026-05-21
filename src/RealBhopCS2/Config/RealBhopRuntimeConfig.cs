namespace RealBhopCS2.Config;

public sealed class RealBhopRuntimeConfig
{
    private bool? _temporaryDebug;

    public RealBhopRuntimeConfig(RealBhopConfig config)
    {
        Config = config;
    }

    public RealBhopConfig Config { get; private set; }

    public bool EffectiveDebug => _temporaryDebug ?? Config.Debug;

    public bool ToggleDebug()
    {
        _temporaryDebug = !EffectiveDebug;
        return EffectiveDebug;
    }

    public void ReplaceConfig(RealBhopConfig config)
    {
        Config = config;
    }
}
