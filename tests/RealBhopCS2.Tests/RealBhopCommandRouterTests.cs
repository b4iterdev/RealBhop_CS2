using RealBhopCS2.Commands;

namespace RealBhopCS2.Tests;

public sealed class RealBhopCommandRouterTests
{
    [Theory]
    [InlineData("status", RealBhopCommandAction.Status)]
    [InlineData("debug", RealBhopCommandAction.Debug)]
    [InlineData("reload", RealBhopCommandAction.Reload)]
    [InlineData("reset", RealBhopCommandAction.Reset)]
    public void Parse_MapsSubcommandsToActions(string subcommand, RealBhopCommandAction expected)
    {
        Assert.Equal(expected, RealBhopCommandRouter.Parse(subcommand));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("unknown")]
    public void Parse_ReturnsHelpForMissingOrUnknownSubcommands(string? subcommand)
    {
        Assert.Equal(RealBhopCommandAction.Help, RealBhopCommandRouter.Parse(subcommand));
    }
}
