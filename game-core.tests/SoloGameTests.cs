using System;

namespace DuskTillDawn.Core.Tests;

public class SoloGameTests
{
    private const double ChallengeDuration = 60;

    private sealed class TestChallenge() : Challenge("Test alert", Room.Servers, points: 100, durationSeconds: ChallengeDuration);

    private static SoloGame NewGame()
    {
        return new SoloGame(new PlayerProfile("Bilal"));
    }

    private static void FailOneChallenge(SoloGame game)
    {
        game.Launch(new TestChallenge());
        game.Tick(ChallengeDuration);
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
    public void Launching_a_challenge_starts_it()
    {
        var game = NewGame();
        var challenge = new TestChallenge();

        game.Launch(challenge);

        Assert.Equal(ChallengeState.Running, challenge.State);
        Assert.Contains(challenge, game.Challenges);
    }

    [Fact]
    public void Two_failures_keep_the_game_going()
    {
        var game = NewGame();

        FailOneChallenge(game);
        FailOneChallenge(game);

        Assert.Equal(2, game.Failures);
        Assert.False(game.IsLost);
        Assert.False(game.IsFinished);
    }

    [Fact]
    public void Three_failures_lose_and_end_the_game()
    {
        var game = NewGame();

        FailOneChallenge(game);
        FailOneChallenge(game);
        FailOneChallenge(game);

        Assert.Equal(3, game.Failures);
        Assert.True(game.IsLost);
        Assert.True(game.IsFinished);
    }

    [Fact]
    public void A_failed_challenge_counts_only_once()
    {
        var game = NewGame();
        FailOneChallenge(game);

        game.Tick(ChallengeDuration);
        game.Tick(ChallengeDuration);

        Assert.Equal(1, game.Failures);
    }

    [Fact]
    public void A_repaired_challenge_is_not_a_failure()
    {
        var game = NewGame();
        var challenge = new TestChallenge();
        game.Launch(challenge);

        challenge.Repair(game.Players[0]);
        game.Tick(ChallengeDuration);

        Assert.Equal(0, game.Failures);
        Assert.Equal(100, game.Players[0].Points);
    }

    [Fact]
    public void Tick_moves_running_challenges_but_not_pending_ones()
    {
        var game = NewGame();
        var running = new TestChallenge();
        var pending = new TestChallenge();
        game.Launch(running);
        game.Add(pending);

        game.Tick(10);

        Assert.Equal(50, running.TimeRemaining);
        Assert.Equal(60, pending.TimeRemaining);
        Assert.Equal(ChallengeState.Pending, pending.State);
    }

    [Fact]
    public void Nothing_moves_after_the_game_is_over()
    {
        var game = NewGame();
        FailOneChallenge(game);
        FailOneChallenge(game);
        FailOneChallenge(game);
        var late = new TestChallenge();
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
    public void A_thousand_ticks_with_ten_challenges_allocate_nothing()
    {
        var game = NewGame();
        for (var i = 0; i < 10; i++)
        {
            game.Launch(new TestChallenge());
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
