using System;
using DuskTillDawn.Core.Incidents;
using DuskTillDawn.Core.Night;

namespace DuskTillDawn.Core.Tests;

public class IncidentSpawnerTests
{
    private const double Interval = 20;

    private static (SoloGame Game, IncidentSpawner Spawner) NewSpawner()
    {
        var game = new SoloGame(new PlayerProfile("Bilal"));
        var spawner = new IncidentSpawner(game, Interval, () => new BrokenDisk(), () => new SecurityAttack());
        return (game, spawner);
    }

    [Fact]
    public void No_incident_before_the_interval()
    {
        var (game, spawner) = NewSpawner();

        spawner.Tick(Interval - 1);

        Assert.Empty(game.Incidents);
    }

    [Fact]
    public void One_incident_after_the_interval()
    {
        var (game, spawner) = NewSpawner();

        spawner.Tick(Interval);

        Assert.Single(game.Incidents);
        Assert.Equal(IncidentState.Running, game.Incidents[0].State);
    }

    [Fact]
    public void Two_incidents_after_twice_the_interval()
    {
        var (game, spawner) = NewSpawner();

        spawner.Tick(Interval);
        spawner.Tick(Interval);

        Assert.Equal(2, game.Incidents.Count);
    }

    [Fact]
    public void Incidents_are_picked_in_turn()
    {
        var (game, spawner) = NewSpawner();

        spawner.Tick(Interval * 2);

        Assert.IsType<BrokenDisk>(game.Incidents[0]);
        Assert.IsType<SecurityAttack>(game.Incidents[1]);
    }

    [Fact]
    public void A_kind_still_running_is_not_launched_again()
    {
        var (game, spawner) = NewSpawner();

        spawner.Tick(Interval * 3);

        Assert.Equal(2, game.Incidents.Count);
    }

    [Fact]
    public void A_fixed_incident_comes_back_only_one_interval_after_its_fix()
    {
        var (game, spawner) = NewSpawner();
        spawner.Tick(Interval * 2);
        spawner.Tick(Interval / 2);
        game.Repair(game.Players[0], PlayerAction.ReplaceDisk);

        spawner.Tick(Interval / 2);
        Assert.Equal(2, game.Incidents.Count);

        spawner.Tick(Interval);
        Assert.Equal(3, game.Incidents.Count);
        Assert.IsType<BrokenDisk>(game.Incidents[2]);
    }

    [Fact]
    public void Many_small_ticks_equal_one_big_tick()
    {
        var (smallGame, smallSpawner) = NewSpawner();
        var (bigGame, bigSpawner) = NewSpawner();

        for (var i = 0; i < 60 * 60; i++)
        {
            smallSpawner.Tick(1.0 / 60.0);
        }
        bigSpawner.Tick(60);

        Assert.Equal(2, bigGame.Incidents.Count);
        Assert.Equal(bigGame.Incidents.Count, smallGame.Incidents.Count);
    }

    [Fact]
    public void No_new_incident_after_the_game_is_over()
    {
        var (game, spawner) = NewSpawner();
        for (var i = 0; i < SoloGame.MaxFailures; i++)
        {
            game.Launch(new BrokenDisk());
            game.Tick(60);
        }
        int countAtGameOver = game.Incidents.Count;

        spawner.Tick(Interval * 5);

        Assert.True(game.IsFinished);
        Assert.Equal(countAtGameOver, game.Incidents.Count);
    }

    [Fact]
    public void A_negative_tick_is_rejected()
    {
        var (_, spawner) = NewSpawner();

        Assert.Throws<ArgumentOutOfRangeException>(() => spawner.Tick(-1));
    }

    [Fact]
    public void The_interval_must_be_positive()
    {
        var game = new SoloGame(new PlayerProfile("Bilal"));

        Assert.Throws<ArgumentOutOfRangeException>(() => new IncidentSpawner(game, 0, () => new BrokenDisk()));
    }

    [Fact]
    public void At_least_one_kind_of_incident_is_needed()
    {
        var game = new SoloGame(new PlayerProfile("Bilal"));

        Assert.Throws<ArgumentException>(() => new IncidentSpawner(game, Interval));
    }

    [Fact]
    public void Without_an_interval_the_spawner_uses_the_default_one()
    {
        var spawner = new IncidentSpawner(new SoloGame(new PlayerProfile("Bilal")), () => new BrokenDisk());

        Assert.Equal(IncidentSpawner.DefaultIntervalSeconds, spawner.IntervalSeconds);
    }
}
