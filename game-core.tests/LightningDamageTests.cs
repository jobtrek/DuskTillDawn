using System;
using System.Linq;

namespace DuskTillDawn.Core.Tests;

public class LightningDamageTests
{
    private const int Seed = 42;
    private const double AlwaysHits = 1;
    private const double NeverHits = 0;

    [Fact]
    public void A_damaged_panel_is_on_the_roof()
    {
        var panel = new DamagedSolarPanel();

        Assert.Equal(Room.Roof, panel.Room);
    }

    [Fact]
    public void Only_repairing_the_panel_fixes_it()
    {
        foreach (var action in Enum.GetValues<PlayerAction>())
        {
            var panel = new DamagedSolarPanel();
            panel.Start();

            panel.Repair(new PlayerProfile("Bilal"), action);

            var expected = action == PlayerAction.RepairPanel ? ChallengeState.Succeeded : ChallengeState.Running;
            Assert.Equal(expected, panel.State);
        }
    }

    [Fact]
    public void A_strike_that_hits_breaks_a_panel()
    {
        var (weather, game, _) = CreateWorld(AlwaysHits);

        weather.ForceStrike();

        var panel = Assert.Single(game.Challenges);
        Assert.IsType<DamagedSolarPanel>(panel);
        Assert.Equal(ChallengeState.Running, panel.State);
    }

    [Fact]
    public void A_strike_that_misses_breaks_nothing()
    {
        var (weather, game, _) = CreateWorld(NeverHits);

        weather.ForceStrike();

        Assert.Empty(game.Challenges);
    }

    [Fact]
    public void A_strike_still_hits_while_another_incident_runs()
    {
        var (weather, game, _) = CreateWorld(AlwaysHits);
        game.Launch(new BrokenDisk());

        weather.ForceStrike();

        Assert.Contains(game.Challenges, challenge => challenge is DamagedSolarPanel { State: ChallengeState.Running });
    }

    [Fact]
    public void Two_strikes_break_two_different_panels()
    {
        var (weather, game, _) = CreateWorld(AlwaysHits);

        weather.ForceStrike();
        weather.ForceStrike();

        var targets = game.Challenges.OfType<DamagedSolarPanel>().Select(panel => panel.Target).ToList();
        Assert.Equal(2, targets.Count);
        Assert.NotEqual(targets[0], targets[1]);
    }

    [Fact]
    public void Once_every_panel_is_broken_a_strike_breaks_nothing_more()
    {
        var (weather, game, panels) = CreateWorld(AlwaysHits);

        for (var i = 0; i < panels.All.Count + 1; i++)
        {
            weather.ForceStrike();
        }

        var targets = game.Challenges.OfType<DamagedSolarPanel>().Select(panel => panel.Target);
        Assert.Equal(panels.All.Count, targets.Distinct().Count());
        Assert.Equal(panels.All.Count, game.Challenges.Count);
    }

    [Fact]
    public void Repairing_one_panel_leaves_the_other_broken()
    {
        var (weather, game, _) = CreateWorld(AlwaysHits);
        weather.ForceStrike();
        weather.ForceStrike();
        var first = (DamagedSolarPanel)game.Challenges[0];
        var second = (DamagedSolarPanel)game.Challenges[1];

        bool repaired = game.RepairPanel(game.Players[0], first.Target!);

        Assert.True(repaired);
        Assert.Equal(ChallengeState.Succeeded, first.State);
        Assert.Equal(ChallengeState.Running, second.State);
    }

    [Fact]
    public void Repairing_a_panel_that_is_not_broken_does_nothing()
    {
        var (weather, game, panels) = CreateWorld(AlwaysHits);
        weather.ForceStrike();
        var broken = ((DamagedSolarPanel)game.Challenges[0]).Target;
        var intact = panels.All.First(panel => panel != broken);

        bool repaired = game.RepairPanel(game.Players[0], intact);

        Assert.False(repaired);
        Assert.Equal(ChallengeState.Running, game.Challenges[0].State);
    }

    [Fact]
    public void Once_the_panel_is_repaired_lightning_can_break_it_again()
    {
        var (weather, game, _) = CreateWorld(AlwaysHits);
        weather.ForceStrike();
        game.Repair(game.Players[0], PlayerAction.RepairPanel);

        weather.ForceStrike();

        Assert.Equal(2, game.Challenges.Count(challenge => challenge is DamagedSolarPanel));
    }

    [Fact]
    public void By_default_some_strikes_hit_and_some_miss()
    {
        Assert.InRange(LightningDamage.DefaultHitChance, 0.01, 0.99);
    }

    private static (WeatherSystem Weather, SoloGame Game, SolarArray Panels) CreateWorld(double hitChance)
    {
        var weather = new WeatherSystem(WeatherKind.Storm, Seed);
        var game = new SoloGame(new PlayerProfile("Bilal"));
        var panels = new SolarArray(new Random(Seed));
        _ = new LightningDamage(weather, game, panels, new Random(Seed), hitChance);
        return (weather, game, panels);
    }
}
