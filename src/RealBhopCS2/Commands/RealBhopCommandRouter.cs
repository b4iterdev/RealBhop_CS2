namespace RealBhopCS2.Commands;

public static class RealBhopCommandRouter
{
    public const string CommandName = "css_realbhop";

    public static RealBhopCommandAction Parse(string? subcommand)
    {
        return subcommand?.Trim().ToLowerInvariant() switch
        {
            "status" => RealBhopCommandAction.Status,
            "debug" => RealBhopCommandAction.Debug,
            "reload" => RealBhopCommandAction.Reload,
            "reset" => RealBhopCommandAction.Reset,
            _ => RealBhopCommandAction.Help
        };
    }
}
