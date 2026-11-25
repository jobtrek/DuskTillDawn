using System;

namespace DuskTillDawn.Core;

public sealed class PlayerProfile(string name)
{
    public string Name { get; } = name;
    public int Points { get; private set; }
    public HeldItem Holding { get; private set; } = HeldItem.None;

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
