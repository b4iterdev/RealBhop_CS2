using CounterStrikeSharp.API.Core;

namespace RealBhopCS2.Config;

public sealed class RealBhopConfig : BasePluginConfig
{
    public bool Enabled { get; set; } = true;

    public int MaxBhopTicks { get; set; } = 12;

    public float FramePenalty { get; set; } = 0.975f;

    public bool ApplyOnlyHorizontalCorrection { get; set; } = true;

    public float MaxCorrectionSpeed { get; set; } = 3500.0f;

    public bool IgnoreBots { get; set; } = true;

    public bool Debug { get; set; }

    public bool SkipTriggerPush { get; set; } = true;

    public bool ExperimentalAirAccelerate { get; set; }

    public float Hl1AirAccelerate { get; set; } = 10.0f;

    public float Hl1MaxSpeed { get; set; } = 320.0f;
}
