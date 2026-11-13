using System;

namespace DuskTillDawn.Core.Tests;

public class PlayerProfileTests
{
    [Fact]
    public void New_player_has_zero_points()
    {
        var player = new PlayerProfile("Bilal");

        Assert.Equal("Bilal", player.Name);
        Assert.Equal(0, player.Points);
    }

    [Fact]
    public void Earning_points_adds_them()
    {
        var player = new PlayerProfile("Bilal");

        player.EarnPoints(150);
        player.EarnPoints(100);

        Assert.Equal(250, player.Points);
    }

    [Fact]
    public void Cannot_earn_negative_points()
    {
        var player = new PlayerProfile("Bilal");

        Assert.Throws<ArgumentOutOfRangeException>(() => player.EarnPoints(-10));
        Assert.Equal(0, player.Points);
    }
}
