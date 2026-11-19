namespace DuskTillDawn.Core.Tests;

public class PointsRuleTests
{
    [Fact]
    public void A_fast_repair_gives_more_than_a_slow_one()
    {
        var fast = new BrokenDisk();
        var slow = new BrokenDisk();
        var fastPlayer = new PlayerProfile("Fast");
        var slowPlayer = new PlayerProfile("Slow");
        fast.Start();
        slow.Start();

        fast.Tick(10);
        slow.Tick(40);
        fast.Repair(fastPlayer, PlayerAction.ReplaceDisk);
        slow.Repair(slowPlayer, PlayerAction.ReplaceDisk);

        Assert.Equal(150, fastPlayer.Points);
        Assert.Equal(100, slowPlayer.Points);
    }

    [Fact]
    public void A_repair_with_exactly_half_the_time_left_is_still_fast()
    {
        var challenge = new BrokenDisk();
        var player = new PlayerProfile("Bilal");
        challenge.Start();

        challenge.Tick(30);
        challenge.Repair(player, PlayerAction.ReplaceDisk);

        Assert.Equal(150, player.Points);
    }

    [Fact]
    public void A_failure_removes_half_the_challenge_points()
    {
        var player = new PlayerProfile("Bilal");
        player.EarnPoints(200);
        var game = new SoloGame(player);
        game.Launch(new BrokenDisk());

        game.Tick(60);

        Assert.Equal(150, player.Points);
    }

    [Fact]
    public void In_a_duo_game_a_failure_costs_both_players()
    {
        var first = new PlayerProfile("Bilal");
        var second = new PlayerProfile("Thomas");
        first.EarnPoints(200);
        second.EarnPoints(200);
        var game = new DuoGame(first, second, challengeCount: 2);
        game.Launch(new SecurityAttack());

        game.Tick(60);

        Assert.Equal(125, first.Points);
        Assert.Equal(125, second.Points);
    }

    [Fact]
    public void Points_never_go_below_zero()
    {
        var player = new PlayerProfile("Bilal");
        var game = new SoloGame(player);
        game.Launch(new SecurityAttack());

        game.Tick(60);

        Assert.Equal(0, player.Points);
    }
}
