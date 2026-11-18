using System;

namespace DuskTillDawn.Core;

/// <summary>Life cycle from docs/game-v1.md §7.</summary>
public enum IncidentState
{
    Dormant,
    Warning,
    Active,
    Escalated,
    Resolved,
}

public enum Room
{
    Roof,
    ControlRoom,
    Basement,
}

public sealed class Incident
{
    private const double RoundingMargin = 1e-9;
    private const double WarningBonus = 1.5;

    private double _secondsInState;

    /// <summary>Timings are starting values (game-v1 §7): Warning → Active after 60 s, Active → Escalated after 3 min.</summary>
    public Incident(string name, Room room, int points, double warningSeconds = 60, double activeSeconds = 180)
    {
        Name = name;
        Room = room;
        Points = points;
        WarningSeconds = warningSeconds;
        ActiveSeconds = activeSeconds;
    }

    public string Name { get; }
    public Room Room { get; }
    public int Points { get; }
    public double WarningSeconds { get; }
    public double ActiveSeconds { get; }
    public IncidentState State { get; private set; } = IncidentState.Dormant;

    public void Start()
    {
        if (State == IncidentState.Dormant)
        {
            State = IncidentState.Warning;
            _secondsInState = 0;
        }
    }

    public void Tick(double seconds)
    {
        if (State != IncidentState.Warning && State != IncidentState.Active)
        {
            return;
        }

        _secondsInState += seconds;

        if (State == IncidentState.Warning && _secondsInState >= WarningSeconds - RoundingMargin)
        {
            State = IncidentState.Active;
            _secondsInState = Math.Max(_secondsInState - WarningSeconds, 0);
        }

        if (State == IncidentState.Active && _secondsInState >= ActiveSeconds - RoundingMargin)
        {
            State = IncidentState.Escalated;
        }
    }

    /// <summary>Resolves the incident and returns the points earned: +50 % when fixed during Warning, 0 if there was nothing to fix.</summary>
    public int Resolve()
    {
        int earned = State switch
        {
            IncidentState.Warning => (int)(Points * WarningBonus),
            IncidentState.Active or IncidentState.Escalated => Points,
            _ => 0,
        };

        if (earned > 0)
        {
            State = IncidentState.Resolved;
        }

        return earned;
    }
}
