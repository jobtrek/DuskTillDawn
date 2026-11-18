namespace DuskTillDawn.Core;

/// <summary>One player. The game is lost after 3 failures.</summary>
public sealed class SoloGame(PlayerProfile player) : Game([player])
{
    public const int MaxFailures = 3;

    public override bool IsLost => Failures >= MaxFailures;
    public override bool IsFinished => IsLost;
}
