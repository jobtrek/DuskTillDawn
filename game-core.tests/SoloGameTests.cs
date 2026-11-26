using System;
using DuskTillDawn.Core.Building;
using DuskTillDawn.Core.Incidents;
using DuskTillDawn.Core.Night;

namespace DuskTillDawn.Core.Tests;

public class SoloGameTests
{
    private const double IncidentDuration = 60;

    private sealed class TestIncident() : Incident("Test alert", Room.Servers, points: 100, durationSeconds: IncidentDuration)
    {
        protected override bool IsRightAction(PlayerAction action)
        {
            return action == PlayerAction.ReplaceDisk;
        }
    }

    private static SoloGame NewGame()
    {
        return new SoloGame(new PlayerProfile("Bilal"));
    }

    private static void FailOneIncident(SoloGame game)
    {
        game.Launch(new TestIncident());
        game.Tick(IncidentDuration);
    }

    [Fact]
    public void A_new_solo_game_has_one_player_and_no_failures()
    {
        var game = NewGame();

        Assert.Single(game.Players);
        Assert.Equal(0, game.Failures);
        Assert.False(game.IsLost);
        Assert.False(game.IsFinished);
    }

    [Fact]
    public void Launching_a_incident_starts_it()
    {
        var game = NewGame();
        var incident = new TestIncident();

        game.Launch(incident);

        Assert.Equal(IncidentState.Running, incident.State);
        Assert.Contains(incident, game.Incidents);
    }

    [Fact]
    public void Two_failures_keep_the_game_going()
    {
        var game = NewGame();

        FailOneIncident(game);
        FailOneIncident(game);

        Assert.Equal(2, game.Failures);
        Assert.False(game.IsLost);
        Assert.False(game.IsFinished);
    }

    [Fact]
    public void Three_failures_lose_and_end_the_game()
    {
        var game = NewGame();

        FailOneIncident(game);
        FailOneIncident(game);
        FailOneIncident(game);

        Assert.Equal(3, game.Failures);
        Assert.True(game.IsLost);
        Assert.True(game.IsFinished);
    }

    [Fact]
    public void A_failed_incident_counts_only_once()
    {
        var game = NewGame();
        FailOneIncident(game);

        game.Tick(IncidentDuration);
        game.Tick(IncidentDuration);

        Assert.Equal(1, game.Failures);
    }

    [Fact]
    public void A_repaired_incident_is_not_a_failure()
    {
        var game = NewGame();
        var incident = new TestIncident();
        game.Launch(incident);

        incident.Repair(game.Players[0], PlayerAction.ReplaceDisk);
        game.Tick(IncidentDuration);

        Assert.Equal(0, game.Failures);
        Assert.Equal(150, game.Players[0].Points);
    }

    [Fact]
    public void Tick_moves_running_incidents_but_not_pending_ones()
    {
        var game = NewGame();
        var running = new TestIncident();
        var pending = new TestIncident();
        game.Launch(running);
        game.Add(pending);

        game.Tick(10);

        Assert.Equal(50, running.TimeRemaining);
        Assert.Equal(60, pending.TimeRemaining);
        Assert.Equal(IncidentState.Pending, pending.State);
    }

    [Fact]
    public void Nothing_moves_after_the_game_is_over()
    {
        var game = NewGame();
        FailOneIncident(game);
        FailOneIncident(game);
        FailOneIncident(game);
        var late = new TestIncident();
        game.Launch(late);

        game.Tick(10);

        Assert.Equal(60, late.TimeRemaining);
        Assert.Equal(3, game.Failures);
    }

    [Fact]
    public void A_negative_tick_is_rejected()
    {
        var game = NewGame();

        Assert.Throws<ArgumentOutOfRangeException>(() => game.Tick(-1));
    }

    [Fact]
    public void A_thousand_ticks_with_ten_incidents_allocate_nothing()
    {
        var game = NewGame();
        for (var i = 0; i < 10; i++)
        {
            game.Launch(new TestIncident());
        }
        game.Tick(0);

        long before = GC.GetAllocatedBytesForCurrentThread();
        for (var i = 0; i < 1000; i++)
        {
            game.Tick(1.0 / 60.0);
        }
        long after = GC.GetAllocatedBytesForCurrentThread();

        Assert.Equal(before, after);
    }
}
