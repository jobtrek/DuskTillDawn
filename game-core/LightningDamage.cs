using System;
using System.Linq;

namespace DuskTillDawn.Core;

public sealed class LightningDamage
{
    public const double DefaultHitChance = 0.35;
    private readonly Random _random;
    private readonly Game _game;
    private readonly double _hitChance;
    private readonly SolarArray _panels;

    public LightningDamage(WeatherSystem weather, Game game, SolarArray panels, Random random, double hitChance = DefaultHitChance)
    {
        _game = game;
        _random = random;
        _hitChance = hitChance;
        _panels = panels;
        weather.LightningStruck += OnStrike;
    }

    private void OnStrike()
    {
        if (_random.NextDouble() >= _hitChance)
        {
            return;
        }

        var broken = _game.Challenges
            .OfType<DamagedSolarPanel>()
            .Where(c => c.State == ChallengeState.Running)
            .Select(c => c.Target!)
            .ToList();

        SolarPanel? panel = _panels.PickRandomExcept(broken);
        if (panel == null)
        {
            return;
        }

        _game.Launch(new DamagedSolarPanel(panel));
    }
}
