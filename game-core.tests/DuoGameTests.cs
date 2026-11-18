using System;
using System.Collections.Generic;

namespace DuskTillDawn.Core.Tests;

public class DuoGameTests
{
    private const double ChallengeDuration = 60;

    private static DuoGame NewGame(int challengeCount = 2)
    {
        return new DuoGame(new PlayerProfile("Bilal"), new PlayerProfile("Thomas"), challengeCount);
    }

    [Fact]
    public void A_new_duo_game_has_two_players()
    {
        var game = NewGame();

        Assert.Equal(2, game.Players.Count);
        Assert.Equal(2, game.ChallengeCount);
        Assert.False(game.IsFinished);
    }

    [Fact]
    public void The_game_is_not_finished_while_a_challenge_is_running()
    {
        var game = NewGame();
        var first = new BrokenDisk();
        var second = new SecurityAttack();
        game.Launch(first);
        game.Launch(second);

        first.Repair(game.Players[0], PlayerAction.ReplaceDisk);

        Assert.False(game.IsFinished);
    }

    [Fact]
    public void The_game_is_not_finished_before_every_challenge_was_played()
    {
        var game = NewGame(challengeCount: 3);
        var challenge = new BrokenDisk();
        game.Launch(challenge);

        challenge.Repair(game.Players[0], PlayerAction.ReplaceDisk);

        Assert.False(game.IsFinished);
    }

    [Fact]
    public void The_game_ends_after_the_last_challenge()
    {
        var game = NewGame();
        var repaired = new BrokenDisk();
        var failed = new SecurityAttack();
        game.Launch(repaired);
        game.Launch(failed);

        repaired.Repair(game.Players[0], PlayerAction.ReplaceDisk);
        game.Tick(ChallengeDuration);

        Assert.True(game.IsFinished);
    }

    [Fact]
    public void The_player_with_more_points_wins()
    {
        var game = NewGame();
        var disk = new BrokenDisk();
        var attack = new SecurityAttack();
        game.Launch(disk);
        game.Launch(attack);

        disk.Repair(game.Players[0], PlayerAction.ReplaceDisk);
        attack.Repair(game.Players[1], PlayerAction.EnableTrafficFilter);

        Assert.True(game.IsFinished);
        Assert.Same(game.Players[1], game.Winner);
    }

    [Fact]
    public void A_tie_gives_no_winner()
    {
        var game = NewGame();
        var first = new BrokenDisk();
        var second = new BrokenDisk();
        game.Launch(first);
        game.Launch(second);

        first.Repair(game.Players[0], PlayerAction.ReplaceDisk);
        second.Repair(game.Players[1], PlayerAction.ReplaceDisk);

        Assert.True(game.IsFinished);
        Assert.Null(game.Winner);
    }

    [Fact]
    public void There_is_no_winner_before_the_end()
    {
        var game = NewGame();
        var challenge = new BrokenDisk();
        game.Launch(challenge);

        challenge.Repair(game.Players[0], PlayerAction.ReplaceDisk);

        Assert.Null(game.Winner);
    }

    [Fact]
    public void A_duo_game_is_never_lost()
    {
        var game = NewGame();
        game.Launch(new BrokenDisk());
        game.Launch(new BrokenDisk());

        game.Tick(ChallengeDuration);

        Assert.Equal(2, game.Failures);
        Assert.True(game.IsFinished);
        Assert.False(game.IsLost);
    }

    [Fact]
    public void The_challenge_count_must_be_positive()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => NewGame(challengeCount: 0));
    }

    [Fact]
    public void Solo_and_duo_games_tick_the_same_way()
    {
        var games = new List<Game> { new SoloGame(new PlayerProfile("Bilal")), NewGame() };

        foreach (var game in games)
        {
            game.Launch(new BrokenDisk());
            game.Tick(ChallengeDuration / 2);
        }

        Assert.All(games, game => Assert.Equal(ChallengeDuration / 2, game.Challenges[0].TimeRemaining));
        Assert.All(games, game => Assert.False(game.IsFinished));
    }
}
