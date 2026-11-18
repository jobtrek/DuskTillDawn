using System;

namespace DuskTillDawn.Core.Tests;

public class ChallengeSpawnerTests
{
    private const double Interval = 20;

    private static (SoloGame Game, ChallengeSpawner Spawner) NewSpawner()
    {
        var game = new SoloGame(new PlayerProfile("Bilal"));
        var spawner = new ChallengeSpawner(game, Interval, () => new BrokenDisk(), () => new SecurityAttack());
        return (game, spawner);
    }

    [Fact]
    public void No_challenge_before_the_interval()
    {
        var (game, spawner) = NewSpawner();

        spawner.Tick(Interval - 1);

        Assert.Empty(game.Challenges);
    }

    [Fact]
    public void One_challenge_after_the_interval()
    {
        var (game, spawner) = NewSpawner();

        spawner.Tick(Interval);

        Assert.Single(game.Challenges);
        Assert.Equal(ChallengeState.Running, game.Challenges[0].State);
    }

    [Fact]
    public void Two_challenges_after_twice_the_interval()
    {
        var (game, spawner) = NewSpawner();

        spawner.Tick(Interval);
        spawner.Tick(Interval);

        Assert.Equal(2, game.Challenges.Count);
    }

    [Fact]
    public void Challenges_are_picked_in_turn()
    {
        var (game, spawner) = NewSpawner();

        spawner.Tick(Interval * 3);

        Assert.IsType<BrokenDisk>(game.Challenges[0]);
        Assert.IsType<SecurityAttack>(game.Challenges[1]);
        Assert.IsType<BrokenDisk>(game.Challenges[2]);
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

        Assert.Equal(3, bigGame.Challenges.Count);
        Assert.Equal(bigGame.Challenges.Count, smallGame.Challenges.Count);
    }

    [Fact]
    public void No_new_challenge_after_the_game_is_over()
    {
        var (game, spawner) = NewSpawner();
        for (var i = 0; i < SoloGame.MaxFailures; i++)
        {
            game.Launch(new BrokenDisk());
            game.Tick(60);
        }
        int countAtGameOver = game.Challenges.Count;

        spawner.Tick(Interval * 5);

        Assert.True(game.IsFinished);
        Assert.Equal(countAtGameOver, game.Challenges.Count);
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

        Assert.Throws<ArgumentOutOfRangeException>(() => new ChallengeSpawner(game, 0, () => new BrokenDisk()));
    }

    [Fact]
    public void At_least_one_kind_of_challenge_is_needed()
    {
        var game = new SoloGame(new PlayerProfile("Bilal"));

        Assert.Throws<ArgumentException>(() => new ChallengeSpawner(game, Interval));
    }
}
