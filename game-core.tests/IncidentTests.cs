using DuskTillDawn.Core.Building;
using DuskTillDawn.Core.Incidents;
using DuskTillDawn.Core.Night;

namespace DuskTillDawn.Core.Tests;

public class IncidentTests
{
    private sealed class TestIncident() : Incident("Test alert", Room.Servers, points: 100, durationSeconds: 60)
    {
        protected override bool IsRightAction(PlayerAction action)
        {
            return action == PlayerAction.ReplaceDisk;
        }
    }

    [Fact]
    public void New_incident_is_pending_with_full_time()
    {
        var incident = new TestIncident();

        Assert.Equal(IncidentState.Pending, incident.State);
        Assert.Equal(60, incident.TimeRemaining);
    }

    [Fact]
    public void Incident_keeps_its_alert_room_and_points()
    {
        var incident = new TestIncident();

        Assert.Equal("Test alert", incident.Alert);
        Assert.Equal(Room.Servers, incident.Room);
        Assert.Equal(100, incident.Points);
        Assert.Equal(60, incident.DurationSeconds);
    }

    [Fact]
    public void Starting_a_incident_makes_it_running()
    {
        var incident = new TestIncident();

        incident.Start();

        Assert.Equal(IncidentState.Running, incident.State);
    }

    [Fact]
    public void Starting_twice_changes_nothing()
    {
        var incident = new TestIncident();
        incident.Start();

        incident.Start();

        Assert.Equal(IncidentState.Running, incident.State);
    }
    [Fact]
    public void Time_does_not_drop_while_pending()
    {
        var incident = new TestIncident();

        incident.Tick(10);

        Assert.Equal(60, incident.TimeRemaining);
        Assert.Equal(IncidentState.Pending, incident.State);
    }

    [Fact]
    public void Time_drops_while_running()
    {
        var incident = new TestIncident();
        incident.Start();

        incident.Tick(10);

        Assert.Equal(50, incident.TimeRemaining);
        Assert.Equal(IncidentState.Running, incident.State);
    }

    [Theory]
    [InlineData(60)]
    [InlineData(90)]
    public void Incident_fails_when_time_reaches_zero(double seconds)
    {
        var incident = new TestIncident();
        incident.Start();

        incident.Tick(seconds);

        Assert.Equal(0, incident.TimeRemaining);
        Assert.Equal(IncidentState.Failed, incident.State);
    }

    [Fact]
    public void Failed_incident_cannot_be_restarted()
    {
        var incident = new TestIncident();
        incident.Start();
        incident.Tick(60);
        incident.Start();
        Assert.Equal(IncidentState.Failed, incident.State);
    }

    [Fact]
    public void Repairing_a_running_incident_succeeds_and_gives_points()
    {
        var incident = new TestIncident();
        var player = new PlayerProfile("Bilal");
        incident.Start();
        incident.Tick(20);

        incident.Repair(player, PlayerAction.ReplaceDisk);

        Assert.Equal(IncidentState.Succeeded, incident.State);
        Assert.Equal(40, incident.TimeRemaining);
        Assert.Equal(150, player.Points);
    }

    [Fact]
    public void Repairing_a_pending_incident_does_nothing()
    {
        var incident = new TestIncident();
        var player = new PlayerProfile("Bilal");

        incident.Repair(player, PlayerAction.ReplaceDisk);

        Assert.Equal(IncidentState.Pending, incident.State);
        Assert.Equal(0, player.Points);
    }

    [Fact]
    public void Failed_incident_gives_no_points()
    {
        var incident = new TestIncident();
        var player = new PlayerProfile("Bilal");
        incident.Start();
        incident.Tick(60);

        incident.Repair(player, PlayerAction.ReplaceDisk);

        Assert.Equal(IncidentState.Failed, incident.State);
        Assert.Equal(0, player.Points);
    }

    [Fact]
    public void Succeeded_incident_gives_points_only_once()
    {
        var incident = new TestIncident();
        var player = new PlayerProfile("Bilal");
        incident.Start();
        incident.Repair(player, PlayerAction.ReplaceDisk);

        incident.Repair(player, PlayerAction.ReplaceDisk);
        incident.Tick(60);

        Assert.Equal(IncidentState.Succeeded, incident.State);
        Assert.Equal(60, incident.TimeRemaining);
        Assert.Equal(150, player.Points);
    }

    [Fact]
    public void Only_first_player_to_repair_gets_points()
    {
        var incident = new TestIncident();
        var first = new PlayerProfile("Bilal");
        var second = new PlayerProfile("Thomas");
        incident.Start();

        incident.Repair(first, PlayerAction.ReplaceDisk);
        incident.Repair(second, PlayerAction.ReplaceDisk);

        Assert.Equal(150, first.Points);
        Assert.Equal(0, second.Points);
    }

    [Fact]
    public void Many_small_ticks_equal_one_big_tick()
    {
        var incident = new TestIncident();
        incident.Start();

        for (var i = 0; i < 60 * 60; i++)
        {
            incident.Tick(1.0 / 60.0);
        }

        Assert.Equal(0, incident.TimeRemaining);
        Assert.Equal(IncidentState.Failed, incident.State);
    }
}
