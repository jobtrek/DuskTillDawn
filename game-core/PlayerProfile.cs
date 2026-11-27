using System;

namespace DuskTillDawn.Core;

public sealed class PlayerProfile(string name)
{
    public string Name { get; } = name;
    public int Points { get; private set; }
    public HeldItem Holding { get; private set; } = HeldItem.None;

    // Time the sysadmin still loses undoing a wrong action; no repair is possible until it runs out.
    private double _busySeconds;

    public bool IsBusy => _busySeconds > 0;
    public double BusySecondsLeft => _busySeconds;

    public void MakeBusy(double seconds)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(seconds);
        _busySeconds = seconds;
    }

    public void Tick(double seconds)
    {
        _busySeconds = Math.Max(_busySeconds - seconds, 0);
    }

    public void EarnPoints(int points)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(points);
        Points += points;
    }

    public void LosePoints(int points)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(points);
        // Points never go below 0: a bad night cannot leave the player in debt.
        Points = Math.Max(Points - points, 0);

    }

    public bool TryPickUp(HeldItem item)
    {
        if (item == HeldItem.None || Holding != HeldItem.None)
        {
            return false;
        }
        Holding = item;
        return true;
    }

    public void Drop()
    {
        Holding = HeldItem.None;
    }
}
