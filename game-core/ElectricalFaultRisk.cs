using System;

namespace DuskTillDawn.Core;

// Electrical faults strike at random, more often when the equipment works hard (big Stage, DDoS, hot room).
// An ignored fault is the only thing that starts a Fire: heat alone shuts the servers down instead.
public sealed class ElectricalFaultRisk
{
    private const double ChancePerInGameMinute = 0.002;
    private const double RealSecondsPerInGameMinute = 50.0 / 60;

    private readonly Game _game;
    private readonly Random _random;

    public ElectricalFaultRisk(Game game, Random? random = null)
    {
        _game = game;
        _random = random ?? new Random();
        _game.ChallengeFailed += challenge =>
        {
            if (challenge is ElectricalFault)
            {
                _game.Launch(new Fire());
            }
        };
    }

    public void Tick(double realSeconds, double powerDraw)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(realSeconds);

        if (_game.IsFinished || _game.IsRunning<ElectricalFault>() || _game.IsRunning<Fire>())
        {
            return;
        }

        double minutes = realSeconds / RealSecondsPerInGameMinute;
        double chancePerMinute = Math.Clamp(ChancePerInGameMinute * powerDraw * powerDraw, 0, 1);
        double chance = 1 - Math.Pow(1 - chancePerMinute, minutes);
        if (_random.NextDouble() < chance)
        {
            _game.Launch(new ElectricalFault());
        }
    }
}
