using System;
using System.Collections.Generic;
using System.Linq;

namespace DuskTillDawn.Core;

public enum ChallengeState
{
    Pending,
    Running,
    // Fixed by the player, but the room still has to get back to normal (e.g. cool down) before it counts.
    Recovering,
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
    RepairPanel,
    CutFaultyCircuit,
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



    // Set by Game when the challenge joins the night; 0 until then.
    public int Id { get; internal set; }
    public string Alert { get; }
    public Room Room { get; }
    public int Points { get; }
    public double DurationSeconds { get; }
    public double TimeRemaining { get; private set; }
    public ChallengeState State { get; private set; } = ChallengeState.Pending;
    private PlayerProfile? _fixedBy;

    // How much SLA the company loses per in-game minute while this challenge is left running.
    public virtual double SlaDrainPerMinute => DefaultSlaDrainPerMinute;

    // How much SLA the company wins back when this challenge is fixed in time.
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

        if (NeedsRecovery)
        {
            _fixedBy = player;
            State = ChallengeState.Recovering;
            return true;
        }

        Succeed(player);
        return true;
    }

    // The timer stopped at the fix, so the fast bonus is counted from that moment, not from when the room recovered.
    public bool TryFinishRecovery(ServerRoom room)
    {
        if (State != ChallengeState.Recovering || _fixedBy is not { } player || !IsRecovered(room))
        {
            return false;
        }

        Succeed(player);
        return true;
    }

    /// <summary>Each challenge decides which action fixes it.</summary>
    protected abstract bool IsRightAction(PlayerAction action);

    // Most fixes count at once; a few (cooling) only count once the room is back to normal.
    protected virtual bool NeedsRecovery => false;

    protected virtual bool IsRecovered(ServerRoom room)
    {
        return true;
    }

    private void Succeed(PlayerProfile player)
    {
        State = ChallengeState.Succeeded;
        player.EarnPoints(PointsRule.RepairReward(Points, TimeRemaining, DurationSeconds));
        Succeeded?.Invoke(this);
    }
}
