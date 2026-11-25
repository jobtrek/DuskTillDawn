using System;
using System.Collections.Generic;

namespace DuskTillDawn.Core;

public enum ChallengeState
{
    Pending,
    Running,
    Succeeded,
    Failed,
}

public enum Room
{
    Roof,
    Control,
    Servers,
}

public enum PlayerAction
{
    EnableTrafficFilter,
    ReplaceDisk,
    RestartCooling,
    UseExtinguisher,
    PlugCable,
    ReportEmail,
}

public abstract class Challenge
{
    private const double RoundingMargin = 1e-9;
    private const double DefaultSlaDrainPerMinute = 0.2;
    private const double DefaultSlaRestoredOnSuccess = 5;

    protected Challenge(string alert, Room room, int points, double durationSeconds)
    {
        Alert = alert;
        Room = room;
        Points = points;
        DurationSeconds = durationSeconds;
        TimeRemaining = durationSeconds;
    }



    // getters and setters allow other classes to modify private properties. 
    public string Alert { get; }
    public Room Room { get; }
    public int Points { get; }
    public double DurationSeconds { get; }
    public double TimeRemaining { get; private set; }
    public ChallengeState State { get; private set; } = ChallengeState.Pending;

    // How much SLA the company loses per in-game minute while this challenge is left running.
    public virtual double SlaDrainPerMinute => DefaultSlaDrainPerMinute;

    // How much SLA the company wins back when this challenge is fixed in time.
    public virtual double SlaRestoredOnSuccess => DefaultSlaRestoredOnSuccess;


    public IReadOnlyList<Server> Targets { get; private set; } = [];

    public void Hit(params Server[] servers)
    {
        Targets = servers;
    }


    public event Action<Challenge>? Succeeded;

    public void Start()
    {
        if (State == ChallengeState.Pending)
        {
            State = ChallengeState.Running;
        }
    }

    public void Tick(double seconds)
    {
        if (State != ChallengeState.Running)
        {
            return;
        }

        TimeRemaining -= seconds;

        if (TimeRemaining <= RoundingMargin)
        {
            TimeRemaining = 0;
            State = ChallengeState.Failed;
        }
    }

    public bool Repair(PlayerProfile player, PlayerAction action)
    {
        if (State != ChallengeState.Running || !IsRightAction(action))
        {
            return false;
        }
        State = ChallengeState.Succeeded;
        player.EarnPoints(PointsRule.RepairReward(Points, TimeRemaining, DurationSeconds));
        Succeeded?.Invoke(this);
        return true;
    }

    /// <summary>Each challenge decides which action fixes it.</summary>
    protected abstract bool IsRightAction(PlayerAction action);
}
