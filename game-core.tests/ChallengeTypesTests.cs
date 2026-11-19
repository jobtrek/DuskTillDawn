using System.Collections.Generic;

namespace DuskTillDawn.Core.Tests;

public class ChallengeTypesTests
{
    [Fact]
    public void Security_attack_has_its_alert_room_and_points()
    {
        var challenge = new SecurityAttack();

        Assert.Equal("Too many suspicious visits!", challenge.Alert);
        Assert.Equal(Room.Control, challenge.Room);
        Assert.Equal(150, challenge.Points);
    }

    [Fact]
    public void Broken_disk_has_its_alert_room_and_points()
    {
        var challenge = new BrokenDisk();

        Assert.Equal("A disk is not responding", challenge.Alert);
        Assert.Equal(Room.Servers, challenge.Room);
        Assert.Equal(100, challenge.Points);
    }

    [Fact]
    public void Enabling_the_traffic_filter_stops_the_security_attack()
    {
        var challenge = new SecurityAttack();
        var player = new PlayerProfile("Bilal");
        challenge.Start();

        challenge.Repair(player, PlayerAction.EnableTrafficFilter);

        Assert.Equal(ChallengeState.Succeeded, challenge.State);
        Assert.Equal(225, player.Points);
    }

    [Fact]
    public void Replacing_the_disk_does_not_stop_the_security_attack()
    {
        var challenge = new SecurityAttack();
        var player = new PlayerProfile("Bilal");
        challenge.Start();

        challenge.Repair(player, PlayerAction.ReplaceDisk);

        Assert.Equal(ChallengeState.Running, challenge.State);
        Assert.Equal(0, player.Points);
    }

    [Fact]
    public void Replacing_the_disk_fixes_the_broken_disk()
    {
        var challenge = new BrokenDisk();
        var player = new PlayerProfile("Bilal");
        challenge.Start();

        challenge.Repair(player, PlayerAction.ReplaceDisk);

        Assert.Equal(ChallengeState.Succeeded, challenge.State);
        Assert.Equal(150, player.Points);
    }

    [Fact]
    public void Enabling_the_traffic_filter_does_not_fix_the_broken_disk()
    {
        var challenge = new BrokenDisk();
        var player = new PlayerProfile("Bilal");
        challenge.Start();

        challenge.Repair(player, PlayerAction.EnableTrafficFilter);

        Assert.Equal(ChallengeState.Running, challenge.State);
        Assert.Equal(0, player.Points);
    }

    [Fact]
    public void A_mixed_list_of_challenges_picks_the_right_action_without_checking_types()
    {
        var player = new PlayerProfile("Bilal");
        var challenges = new List<Challenge> { new SecurityAttack(), new BrokenDisk() };
        var actions = new[] { PlayerAction.EnableTrafficFilter, PlayerAction.ReplaceDisk };

        foreach (var challenge in challenges)
        {
            challenge.Start();
            foreach (var action in actions)
            {
                challenge.Repair(player, action);
            }
        }

        Assert.All(challenges, challenge => Assert.Equal(ChallengeState.Succeeded, challenge.State));
        Assert.Equal(375, player.Points);
    }
}
