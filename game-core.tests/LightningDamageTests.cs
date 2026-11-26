using System;
using System.Linq;
using DuskTillDawn.Core.Building;
using DuskTillDawn.Core.Incidents;
using DuskTillDawn.Core.Night;
using DuskTillDawn.Core.Weather;

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

            var expected = action == PlayerAction.RepairPanel ? IncidentState.Succeeded : IncidentState.Running;
            Assert.Equal(expected, panel.State);
        }
    }

    [Fact]
    public void A_strike_that_hits_breaks_a_panel()
    {
        var (weather, game) = CreateWorld(AlwaysHits);

        weather.ForceStrike();

        var panel = Assert.Single(game.Incidents);
        Assert.IsType<DamagedSolarPanel>(panel);
        Assert.Equal(IncidentState.Running, panel.State);
    }

    [Fact]
    public void A_strike_that_misses_breaks_nothing()
    {
        var (weather, game) = CreateWorld(NeverHits);

        weather.ForceStrike();

        Assert.Empty(game.Incidents);
    }

    [Fact]
    public void A_strike_waits_while_another_incident_runs()
    {
        var (weather, game) = CreateWorld(AlwaysHits);
        game.Launch(new BrokenDisk());

        weather.ForceStrike();

        Assert.DoesNotContain(game.Incidents, incident => incident is DamagedSolarPanel);
    }

    [Fact]
    public void Two_strikes_in_a_row_break_only_one_panel()
    {
        var (weather, game) = CreateWorld(AlwaysHits);

        weather.ForceStrike();
        weather.ForceStrike();

        Assert.Single(game.Incidents);
    }

    [Fact]
    public void Once_the_panel_is_repaired_lightning_can_break_it_again()
    {
        var (weather, game) = CreateWorld(AlwaysHits);
        weather.ForceStrike();
        game.Repair(game.Players[0], PlayerAction.RepairPanel);

        weather.ForceStrike();

        Assert.Equal(2, game.Incidents.Count(incident => incident is DamagedSolarPanel));
    }

    [Fact]
    public void By_default_some_strikes_hit_and_some_miss()
    {
        Assert.InRange(LightningDamage.DefaultHitChance, 0.01, 0.99);
    }

    private static (WeatherSystem Weather, SoloGame Game) CreateWorld(double hitChance)
    {
        var weather = new WeatherSystem(WeatherKind.Storm, Seed);
        var game = new SoloGame(new PlayerProfile("Bilal"));
        _ = new LightningDamage(weather, game, new Random(Seed), hitChance);
        return (weather, game);
    }
}
