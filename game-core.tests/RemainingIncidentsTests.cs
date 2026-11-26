using System;
using System.Collections.Generic;
using DuskTillDawn.Core.Building;
using DuskTillDawn.Core.Incidents;
using DuskTillDawn.Core.Night;

namespace DuskTillDawn.Core.Tests;

public class RemainingIncidentsTests
{
    public static TheoryData<string, string, Room, int> Descriptions => new()
    {
        { nameof(OverheatingServer), "Temperature too high", Room.Servers, 100 },
        { nameof(Fire), "Fire alarm!", Room.Servers, 200 },
        { nameof(UnpluggedCable), "A server is offline", Room.Servers, 100 },
        { nameof(PhishingEmail), "New urgent message", Room.Control, 100 },
    };

    public static TheoryData<string, PlayerAction> RightActions => new()
    {
        { nameof(OverheatingServer), PlayerAction.RestartCooling },
        { nameof(Fire), PlayerAction.UseExtinguisher },
        { nameof(UnpluggedCable), PlayerAction.PlugCable },
        { nameof(PhishingEmail), PlayerAction.ReportEmail },
    };

    [Theory]
    [MemberData(nameof(Descriptions))]
    public void Each_incident_has_its_alert_room_and_points(string name, string alert, Room room, int points)
    {
        var incident = Create(name);

        Assert.Equal(alert, incident.Alert);
        Assert.Equal(room, incident.Room);
        Assert.Equal(points, incident.Points);
    }

    [Theory]
    [MemberData(nameof(RightActions))]
    public void Only_its_own_action_fixes_the_incident(string name, PlayerAction rightAction)
    {
        foreach (var action in Enum.GetValues<PlayerAction>())
        {
            var incident = Create(name);
            var player = new PlayerProfile("Bilal");
            incident.Start();

            incident.Repair(player, action);

            if (action == rightAction)
            {
                Assert.Equal(IncidentState.Succeeded, incident.State);
                // Repaired at once, so the fast repair bonus applies.
                Assert.Equal(incident.Points + incident.Points * PointsRule.FastRepairBonusPercent / 100, player.Points);
            }
            else
            {
                Assert.Equal(IncidentState.Running, incident.State);
                Assert.Equal(0, player.Points);
            }
        }
    }

    [Fact]
    public void All_six_incidents_get_fixed_by_trying_every_action_without_checking_types()
    {
        var player = new PlayerProfile("Bilal");
        var incidents = new List<Incident>
        {
            new SecurityAttack(), new BrokenDisk(), new OverheatingServer(),
            new Fire(), new UnpluggedCable(), new PhishingEmail(),
        };

        foreach (var incident in incidents)
        {
            incident.Start();
            foreach (var action in Enum.GetValues<PlayerAction>())
            {
                incident.Repair(player, action);
            }
        }

        Assert.All(incidents, incident => Assert.Equal(IncidentState.Succeeded, incident.State));
        Assert.Equal((150 + 100 + 100 + 200 + 100 + 100) * 3 / 2, player.Points);
    }

    private static Incident Create(string name)
    {
        return name switch
        {
            nameof(OverheatingServer) => new OverheatingServer(),
            nameof(Fire) => new Fire(),
            nameof(UnpluggedCable) => new UnpluggedCable(),
            nameof(PhishingEmail) => new PhishingEmail(),
            _ => throw new ArgumentException($"Unknown incident {name}", nameof(name)),
        };
    }
}
