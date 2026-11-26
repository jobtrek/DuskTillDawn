using System;
using System.Collections.Generic;
using System.Linq;
using DuskTillDawn.Core.Building;
using DuskTillDawn.Core.Mails;
using DuskTillDawn.Core.Night;
using DuskTillDawn.Core.Servers;

namespace DuskTillDawn.Core.Incidents;

public abstract class Incident
{
    private const double RoundingMargin = 1e-9;
    private const double DefaultSlaDrainPerMinute = 0.2;
    private const double DefaultSlaRestoredOnSuccess = 5;

    protected Incident(string alert, Room room, int points, double durationSeconds)
    {
        Alert = alert;
        Room = room;
        Points = points;
        DurationSeconds = durationSeconds;
        TimeRemaining = durationSeconds;
    }



    // Set by Game when the incident joins the night; 0 until then.
    public int Id { get; internal set; }
    public string Alert { get; }
    public Room Room { get; }
    public int Points { get; }
    public double DurationSeconds { get; }
    public double TimeRemaining { get; private set; }
    public IncidentState State { get; private set; } = IncidentState.Pending;

    // How much SLA the company loses per in-game minute while this incident is left running.
    public virtual double SlaDrainPerMinute => DefaultSlaDrainPerMinute;

    // How much SLA the company wins back when this incident is fixed in time.
    public virtual double SlaRestoredOnSuccess => DefaultSlaRestoredOnSuccess;


    public IReadOnlyList<Server> Targets { get; private set; } = [];

    // The Warning mail the inbox gets when this Incident starts; null when it sends none.
    public virtual WarningText? Warning => null;

    protected string TargetNames(string whenNone)
    {
        return Targets.Count == 0 ? whenNone : string.Join(", ", Targets.Select(server => server.Name));
    }

    public void Hit(params Server[] servers)
    {
        Targets = servers;
    }


    public event Action<Incident>? Succeeded;

    public void Start()
    {
        if (State == IncidentState.Pending)
        {
            State = IncidentState.Running;
        }
    }

    public void Tick(double seconds)
    {
        if (State != IncidentState.Running)
        {
            return;
        }

        TimeRemaining -= seconds;

        if (TimeRemaining <= RoundingMargin)
        {
            TimeRemaining = 0;
            State = IncidentState.Failed;
        }
    }

    public bool Repair(PlayerProfile player, PlayerAction action)
    {
        if (State != IncidentState.Running || !IsRightAction(action))
        {
            return false;
        }
        State = IncidentState.Succeeded;
        player.EarnPoints(PointsRule.RepairReward(Points, TimeRemaining, DurationSeconds));
        Succeeded?.Invoke(this);
        return true;
    }

    /// <summary>Each incident decides which action fixes it.</summary>
    protected abstract bool IsRightAction(PlayerAction action);
}
