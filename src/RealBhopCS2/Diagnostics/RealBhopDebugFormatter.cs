using System.Globalization;
using RealBhopCS2.Config;
using RealBhopCS2.Movement;
using RealBhopCS2.Tracking;

namespace RealBhopCS2.Diagnostics;

public static class RealBhopDebugFormatter
{
    public static string FormatStatus(RealBhopRuntimeConfig runtimeConfig, int trackedPlayers)
    {
        var config = runtimeConfig.Config;

        return string.Join(
            "\n",
            $"RealBhop: enabled={config.Enabled}",
            $"MaxBhopTicks={config.MaxBhopTicks}",
            $"FramePenalty={config.FramePenalty.ToString("0.###", CultureInfo.InvariantCulture)}",
            $"Debug={runtimeConfig.EffectiveDebug}",
            $"ConfigDebug={config.Debug}",
            $"PlayersTracked={trackedPlayers}");
    }

    public static string FormatCorrection(string playerName, MovementSnapshot snapshot, PlayerBhopState state, BhopTickResult result)
    {
        return $"[RealBhop] player={playerName} tick={snapshot.Tick} groundTicks={state.GroundTicks} " +
               $"lastAir={state.LastAirVelocity.HorizontalLength.ToString("0.##", CultureInfo.InvariantCulture)} current={snapshot.Velocity.HorizontalLength.ToString("0.##", CultureInfo.InvariantCulture)} " +
               $"correction={result.Correction.HorizontalLength.ToString("0.##", CultureInfo.InvariantCulture)} reason={result.Reason}";
    }
}
