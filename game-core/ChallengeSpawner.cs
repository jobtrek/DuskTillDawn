using System;

namespace DuskTillDawn.Core;

// Keeps the player busy: a new challenge every interval, the kinds taken in turn.
// A kind never runs twice at once, and a kind that just ended waits one full interval before it comes back,
// so a fixed incident does not reappear right after the player fixed it.
public sealed class ChallengeSpawner
{
    public const double DefaultIntervalSeconds = 20;
    private const double RoundingMargin = 1e-9;

    private readonly Game _game;
    private readonly Func<Challenge>[] _kinds;
    private readonly Challenge?[] _lastOfKind;
    private readonly double[] _lastEndedAt;
    private double _elapsedSeconds;
    private double _secondsSinceLast;
    private int _nextKind;

    public ChallengeSpawner(Game game, params Func<Challenge>[] kinds)
        : this(game, DefaultIntervalSeconds, kinds)
    {
    }

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
        _lastOfKind = new Challenge?[kinds.Length];
        _lastEndedAt = new double[kinds.Length];
    }

    public double IntervalSeconds { get; }

    public void Tick(double seconds)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(seconds);

        if (_game.IsFinished)
        {
            return;
        }

        NoteEndedChallenges();
        _elapsedSeconds += seconds;
        _secondsSinceLast += seconds;

        // A big tick can cover several intervals: one launch per interval at most.
        while (_secondsSinceLast >= IntervalSeconds - RoundingMargin)
        {
            _secondsSinceLast -= IntervalSeconds;
            LaunchNextAvailableKind(_elapsedSeconds - _secondsSinceLast);
        }
    }

    private void NoteEndedChallenges()
    {
        for (var i = 0; i < _kinds.Length; i++)
        {
            if (_lastOfKind[i] is { State: ChallengeState.Succeeded or ChallengeState.Failed } && double.IsNaN(_lastEndedAt[i]))
            {
                _lastEndedAt[i] = _elapsedSeconds;
            }
        }
    }

    private void LaunchNextAvailableKind(double now)
    {
        for (var offset = 0; offset < _kinds.Length; offset++)
        {
            int kind = (_nextKind + offset) % _kinds.Length;
            if (!IsAvailable(kind, now))
            {
                continue;
            }

            Challenge challenge = _kinds[kind]();
            _lastOfKind[kind] = challenge;
            _lastEndedAt[kind] = double.NaN;
            _game.Launch(challenge);
            _nextKind = (kind + 1) % _kinds.Length;
            return;
        }
    }

    private bool IsAvailable(int kind, double now)
    {
        Challenge? last = _lastOfKind[kind];
        if (last == null)
        {
            return true;
        }

        return !double.IsNaN(_lastEndedAt[kind]) && now - _lastEndedAt[kind] >= IntervalSeconds - RoundingMargin;
    }
}
