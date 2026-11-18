using System;

namespace DuskTillDawn.Core.Tests;

public class PlayerProfileTests
{
    [Fact]
    public void New_player_has_zero_points_in_the_control_room()
    {
        var player = new PlayerProfile("Bilal");

        Assert.Equal("Bilal", player.Name);
        Assert.Equal(0, player.Points);
        Assert.Equal(Room.ControlRoom, player.Room);
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

    [Fact]
    public void Fixing_in_the_right_room_resolves_and_gives_the_player_points()
    {
        var player = new PlayerProfile("Bilal");
        var incident = new Incident("Failing hard drive", Room.Basement, points: 100);
        incident.Start();
        player.MoveTo(Room.Basement);

        Assert.True(player.TryFix(incident));

        Assert.Equal(IncidentState.Resolved, incident.State);
        Assert.Equal(150, player.Points);
    }

    [Fact]
    public void Cannot_fix_from_another_room()
    {
        var player = new PlayerProfile("Bilal");
        var incident = new Incident("Failing hard drive", Room.Basement, points: 100);
        incident.Start();

        Assert.False(player.TryFix(incident));

        Assert.Equal(IncidentState.Warning, incident.State);
        Assert.Equal(0, player.Points);
    }
}
