using System;
using System.Linq;

namespace DuskTillDawn.Core;

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

        if (_game.Challenges.Any(c => c.State == ChallengeState.Running))
        {
            return;
        }

        _game.Launch(new DamagedSolarPanel());
    }
}
