using System;

namespace DuskTillDawn.Core.Incidents;

// Keeps the player busy: a new incident every interval, the kinds taken in turn.
// A kind never runs twice at once, and a kind that just ended waits one full interval before it comes back,
// so a fixed incident does not reappear right after the player fixed it.
// The game gives it a single kind that picks an incident at random, so only one spawned incident runs at a time
// (a Fake e-mail from a Phishing mail can run beside it). v1 lets two incidents overlap only on the final night.
public sealed class IncidentSpawner
{
    public const double DefaultIntervalSeconds = 20;
    private const double RoundingMargin = 1e-9;

    private readonly Game _game;
    private readonly Func<Incident>[] _kinds;
    private readonly Incident?[] _lastOfKind;
    private readonly double[] _lastEndedAt;
    private double _elapsedSeconds;
    private double _secondsSinceLast;
    private int _nextKind;

    public IncidentSpawner(Game game, params Func<Incident>[] kinds)
        : this(game, DefaultIntervalSeconds, kinds)
    {
    }

    public IncidentSpawner(Game game, double intervalSeconds, params Func<Incident>[] kinds)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(intervalSeconds);
        if (kinds.Length == 0)
        {
            throw new ArgumentException("Give at least one kind of incident.", nameof(kinds));
        }

        _game = game;
        IntervalSeconds = intervalSeconds;
        _kinds = kinds;
        _lastOfKind = new Incident?[kinds.Length];
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

        NoteEndedIncidents();
        _elapsedSeconds += seconds;
        _secondsSinceLast += seconds;

        // A big tick can cover several intervals: one launch per interval at most.
        while (_secondsSinceLast >= IntervalSeconds - RoundingMargin)
        {
            _secondsSinceLast -= IntervalSeconds;
            LaunchNextAvailableKind(_elapsedSeconds - _secondsSinceLast);
        }
    }

    private void NoteEndedIncidents()
    {
        for (var i = 0; i < _kinds.Length; i++)
        {
            if (_lastOfKind[i] is { State: IncidentState.Succeeded or IncidentState.Failed } && double.IsNaN(_lastEndedAt[i]))
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

            Incident incident = _kinds[kind]();
            _lastOfKind[kind] = incident;
            _lastEndedAt[kind] = double.NaN;
            _game.Launch(incident);
            _nextKind = (kind + 1) % _kinds.Length;
            return;
        }
    }

    private bool IsAvailable(int kind, double now)
    {
        Incident? last = _lastOfKind[kind];
        if (last == null)
        {
            return true;
        }

        return !double.IsNaN(_lastEndedAt[kind]) && now - _lastEndedAt[kind] >= IntervalSeconds - RoundingMargin;
    }
}
