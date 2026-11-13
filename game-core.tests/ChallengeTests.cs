namespace DuskTillDawn.Core.Tests;

public class ChallengeTests
{
    private sealed class TestChallenge() : Challenge("Test alert", Room.Servers, points: 100, durationSeconds: 60);
    [Fact]
    public void New_challenge_is_pending_with_full_time()
    {
        var challenge = new TestChallenge();

        Assert.Equal(ChallengeState.Pending, challenge.State);
        Assert.Equal(60, challenge.TimeRemaining);
    }

    [Fact]
    public void Challenge_keeps_its_alert_room_and_points()
    {
        var challenge = new TestChallenge();

        Assert.Equal("Test alert", challenge.Alert);
        Assert.Equal(Room.Servers, challenge.Room);
        Assert.Equal(100, challenge.Points);
        Assert.Equal(60, challenge.DurationSeconds);
    }

    [Fact]
    public void Starting_a_challenge_makes_it_running()
    {
        var challenge = new TestChallenge();

        challenge.Start();

        Assert.Equal(ChallengeState.Running, challenge.State);
    }

    [Fact]
    public void Starting_twice_changes_nothing()
    {
        var challenge = new TestChallenge();
        challenge.Start();

        challenge.Start();

        Assert.Equal(ChallengeState.Running, challenge.State);
    }
    [Fact]
    public void Time_does_not_drop_while_pending()
    {
        var challenge = new TestChallenge();

        challenge.Tick(10);

        Assert.Equal(60, challenge.TimeRemaining);
        Assert.Equal(ChallengeState.Pending, challenge.State);
    }

    [Fact]
    public void Time_drops_while_running()
    {
        var challenge = new TestChallenge();
        challenge.Start();

        challenge.Tick(10);

        Assert.Equal(50, challenge.TimeRemaining);
        Assert.Equal(ChallengeState.Running, challenge.State);
    }

    [Theory]
    [InlineData(60)]
    [InlineData(90)]
    public void Challenge_fails_when_time_reaches_zero(double seconds)
    {
        var challenge = new TestChallenge();
        challenge.Start();

        challenge.Tick(seconds);

        Assert.Equal(0, challenge.TimeRemaining);
        Assert.Equal(ChallengeState.Failed, challenge.State);
    }

    [Fact]
    public void Failed_challenge_cannot_be_restarted()
    {
        var challenge = new TestChallenge();
        challenge.Start();
        challenge.Tick(60);
        challenge.Start();
        Assert.Equal(ChallengeState.Failed, challenge.State);
    }

    [Fact]
    public void Repairing_a_running_challenge_succeeds_and_gives_points()
    {
        var challenge = new TestChallenge();
        var player = new PlayerProfile("Bilal");
        challenge.Start();
        challenge.Tick(20);

        challenge.Repair(player);

        Assert.Equal(ChallengeState.Succeeded, challenge.State);
        Assert.Equal(40, challenge.TimeRemaining);
        Assert.Equal(100, player.Points);
    }

    [Fact]
    public void Repairing_a_pending_challenge_does_nothing()
    {
        var challenge = new TestChallenge();
        var player = new PlayerProfile("Bilal");

        challenge.Repair(player);

        Assert.Equal(ChallengeState.Pending, challenge.State);
        Assert.Equal(0, player.Points);
    }

    [Fact]
    public void Failed_challenge_gives_no_points()
    {
        var challenge = new TestChallenge();
        var player = new PlayerProfile("Bilal");
        challenge.Start();
        challenge.Tick(60);

        challenge.Repair(player);

        Assert.Equal(ChallengeState.Failed, challenge.State);
        Assert.Equal(0, player.Points);
    }

    [Fact]
    public void Succeeded_challenge_gives_points_only_once()
    {
        var challenge = new TestChallenge();
        var player = new PlayerProfile("Bilal");
        challenge.Start();
        challenge.Repair(player);

        challenge.Repair(player);
        challenge.Tick(60);

        Assert.Equal(ChallengeState.Succeeded, challenge.State);
        Assert.Equal(60, challenge.TimeRemaining);
        Assert.Equal(100, player.Points);
    }

    [Fact]
    public void Only_first_player_to_repair_gets_points()
    {
        var challenge = new TestChallenge();
        var first = new PlayerProfile("Bilal");
        var second = new PlayerProfile("Thomas");
        challenge.Start();

        challenge.Repair(first);
        challenge.Repair(second);

        Assert.Equal(100, first.Points);
        Assert.Equal(0, second.Points);
    }

    [Fact]
    public void Many_small_ticks_equal_one_big_tick()
    {
        var challenge = new TestChallenge();
        challenge.Start();

        for (var i = 0; i < 60 * 60; i++)
        {
            challenge.Tick(1.0 / 60.0);
        }

        Assert.Equal(0, challenge.TimeRemaining);
        Assert.Equal(ChallengeState.Failed, challenge.State);
    }
}
