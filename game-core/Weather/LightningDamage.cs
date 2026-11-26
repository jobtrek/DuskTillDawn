using System;
using System.Linq;
using DuskTillDawn.Core.Incidents;

namespace DuskTillDawn.Core.Weather;

public sealed class LightningDamage
{
    public const double DefaultHitChance = 0.35;
    private readonly Random _random;
    private readonly Game _game;
    private readonly double _hitChance;

    public LightningDamage(WeatherSystem weather, Game game, Random random, double hitChance = DefaultHitChance)
    {
        _game = game;
        _random = random;
        _hitChance = hitChance;
        weather.LightningStruck += OnStrike;
    }

    private void OnStrike()
    {
        if (_random.NextDouble() >= _hitChance)
        {
            return;
        }

        if (_game.Incidents.Any(c => c.State == IncidentState.Running))
        {
            return;
        }

        _game.Launch(new DamagedSolarPanel());
    }
}
