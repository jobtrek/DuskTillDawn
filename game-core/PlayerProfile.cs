using System;

namespace DuskTillDawn.Core;

/// <summary>Player actions go through here (contract §3.1). Each player keeps their own points.</summary>
public sealed class PlayerProfile(string name)
{
    public string Name { get; } = name;
    public int Points { get; private set; }
    public Room Room { get; private set; } = Room.ControlRoom;

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

    public void MoveTo(Room room)
    {
        Room = room;
    }

    /// <summary>Fixes the incident if the player stands in its room (game-v1 §4). Returns true when it was resolved.</summary>
    public bool TryFix(Incident incident)
    {
        if (incident.Room != Room)
        {
            return false;
        }

        int earned = incident.Resolve();
        if (earned == 0)
        {
            return false;
        }

        EarnPoints(earned);
        return true;
    }
}
