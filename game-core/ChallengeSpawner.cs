using System;

namespace DuskTillDawn.Core;

/// <summary>
/// Launches a new challenge every few seconds, picking the kinds in turn.
/// Driven by Tick like the game: no timer of its own.
/// </summary>
public sealed class ChallengeSpawner
{
    public const double DefaultIntervalSeconds = 20;
    private const double RoundingMargin = 1e-9;

    private readonly Game _game;
    private readonly Func<Challenge>[] _kinds;
    private double _secondsSinceLast;
    private int _nextKind;

    public ChallengeSpawner(Game game, double intervalSeconds, params Func<Challenge>[] kinds)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(intervalSeconds);
        if (kinds.Length == 0)
        {
            throw new ArgumentException("Give at least one kind of challenge.", nameof(kinds));
        }

        _game = game;
        IntervalSeconds = intervalSeconds;
        _kinds = kinds;
    }

    public double IntervalSeconds { get; }

    public void Tick(double seconds)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(seconds);

        if (_game.IsFinished)
        {
            return;
        }

        _secondsSinceLast += seconds;

        // A big tick can cover several intervals: launch one challenge per interval.
        while (_secondsSinceLast >= IntervalSeconds - RoundingMargin)
        {
            _secondsSinceLast -= IntervalSeconds;
            _game.Launch(_kinds[_nextKind]());
            _nextKind = (_nextKind + 1) % _kinds.Length;
        }
    }
}
