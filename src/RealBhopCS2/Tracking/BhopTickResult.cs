using RealBhopCS2.Movement;

namespace RealBhopCS2.Tracking;

public sealed record BhopTickResult(
    bool ShouldApplyCorrection,
    MovementVector Correction,
    string Reason)
{
    public static BhopTickResult None(string reason) => new(false, default, reason);

    public static BhopTickResult Apply(MovementVector correction) => new(true, correction, "CorrectionApplied");
}
