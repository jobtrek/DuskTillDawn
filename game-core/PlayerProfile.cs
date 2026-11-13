using System;
using System.Drawing;

namespace DuskTillDawn.Core;

public sealed class PlayerProfile(string name)
{
    public string Name { get; } = name;
    public int Points { get; private set; }

    public void EarnPoints(int points)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(points);
        Points += points;
    }

    public void LosePoints(int points)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(points);
        Points -= points;

    }
}
