using System;

namespace DuskTillDawn.Core;

public sealed class Joueur(string nom)
{
    public string Nom { get; } = nom;
    public int Points { get; private set; }

    public void GagnerPoints(int points)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(points);
        Points += points;
    }
}
