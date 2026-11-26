using System.Collections.Generic;
using DuskTillDawn.Core.Building;
using DuskTillDawn.Core.Incidents;
using DuskTillDawn.Core.Night;

namespace DuskTillDawn.Core.Tests;

public class IncidentTypesTests
{
    [Fact]
    public void Security_attack_has_its_alert_room_and_points()
    {
        var incident = new SecurityAttack();

        Assert.Equal("Too many suspicious visits!", incident.Alert);
        Assert.Equal(Room.Control, incident.Room);
        Assert.Equal(150, incident.Points);
    }

    [Fact]
    public void Broken_disk_has_its_alert_room_and_points()
    {
        var incident = new BrokenDisk();

        Assert.Equal("A disk is not responding", incident.Alert);
        Assert.Equal(Room.Servers, incident.Room);
        Assert.Equal(100, incident.Points);
    }

    [Fact]
    public void Enabling_the_traffic_filter_stops_the_security_attack()
    {
        var incident = new SecurityAttack();
        var player = new PlayerProfile("Bilal");
        incident.Start();

        incident.Repair(player, PlayerAction.EnableTrafficFilter);

        Assert.Equal(IncidentState.Succeeded, incident.State);
        Assert.Equal(225, player.Points);
    }

    [Fact]
    public void Replacing_the_disk_does_not_stop_the_security_attack()
    {
        var incident = new SecurityAttack();
        var player = new PlayerProfile("Bilal");
        incident.Start();

        incident.Repair(player, PlayerAction.ReplaceDisk);

        Assert.Equal(IncidentState.Running, incident.State);
        Assert.Equal(0, player.Points);
    }

    [Fact]
    public void Replacing_the_disk_fixes_the_broken_disk()
    {
        var incident = new BrokenDisk();
        var player = new PlayerProfile("Bilal");
        incident.Start();

        incident.Repair(player, PlayerAction.ReplaceDisk);

        Assert.Equal(IncidentState.Succeeded, incident.State);
        Assert.Equal(150, player.Points);
    }

    [Fact]
    public void Enabling_the_traffic_filter_does_not_fix_the_broken_disk()
    {
        var incident = new BrokenDisk();
        var player = new PlayerProfile("Bilal");
        incident.Start();

        incident.Repair(player, PlayerAction.EnableTrafficFilter);

        Assert.Equal(IncidentState.Running, incident.State);
        Assert.Equal(0, player.Points);
    }

    [Fact]
    public void A_mixed_list_of_incidents_picks_the_right_action_without_checking_types()
    {
        var player = new PlayerProfile("Bilal");
        var incidents = new List<Incident> { new SecurityAttack(), new BrokenDisk() };
        var actions = new[] { PlayerAction.EnableTrafficFilter, PlayerAction.ReplaceDisk };

        foreach (var incident in incidents)
        {
            incident.Start();
            foreach (var action in actions)
            {
                incident.Repair(player, action);
            }
        }

        Assert.All(incidents, incident => Assert.Equal(IncidentState.Succeeded, incident.State));
        Assert.Equal(375, player.Points);
    }
}
