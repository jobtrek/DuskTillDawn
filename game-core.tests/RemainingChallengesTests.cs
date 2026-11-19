using System;
using System.Collections.Generic;

namespace DuskTillDawn.Core.Tests;

public class RemainingChallengesTests
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
    public void Each_challenge_has_its_alert_room_and_points(string name, string alert, Room room, int points)
    {
        var challenge = Create(name);

        Assert.Equal(alert, challenge.Alert);
        Assert.Equal(room, challenge.Room);
        Assert.Equal(points, challenge.Points);
    }

    [Theory]
    [MemberData(nameof(RightActions))]
    public void Only_its_own_action_fixes_the_challenge(string name, PlayerAction rightAction)
    {
        foreach (var action in Enum.GetValues<PlayerAction>())
        {
            var challenge = Create(name);
            var player = new PlayerProfile("Bilal");
            challenge.Start();

            challenge.Repair(player, action);

            if (action == rightAction)
            {
                Assert.Equal(ChallengeState.Succeeded, challenge.State);
                // Repaired at once, so the fast repair bonus applies.
                Assert.Equal(challenge.Points + challenge.Points * PointsRule.FastRepairBonusPercent / 100, player.Points);
            }
            else
            {
                Assert.Equal(ChallengeState.Running, challenge.State);
                Assert.Equal(0, player.Points);
            }
        }
    }

    [Fact]
    public void All_six_challenges_get_fixed_by_trying_every_action_without_checking_types()
    {
        var player = new PlayerProfile("Bilal");
        var challenges = new List<Challenge>
        {
            new SecurityAttack(), new BrokenDisk(), new OverheatingServer(),
            new Fire(), new UnpluggedCable(), new PhishingEmail(),
        };

        foreach (var challenge in challenges)
        {
            challenge.Start();
            foreach (var action in Enum.GetValues<PlayerAction>())
            {
                challenge.Repair(player, action);
            }
        }

        Assert.All(challenges, challenge => Assert.Equal(ChallengeState.Succeeded, challenge.State));
        Assert.Equal((150 + 100 + 100 + 200 + 100 + 100) * 3 / 2, player.Points);
    }

    private static Challenge Create(string name)
    {
        return name switch
        {
            nameof(OverheatingServer) => new OverheatingServer(),
            nameof(Fire) => new Fire(),
            nameof(UnpluggedCable) => new UnpluggedCable(),
            nameof(PhishingEmail) => new PhishingEmail(),
            _ => throw new ArgumentException($"Unknown challenge {name}", nameof(name)),
        };
    }
}
