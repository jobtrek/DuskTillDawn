namespace DuskTillDawn.Core;

/// <summary>Too much suspicious traffic. Fixed from the control room computer.</summary>
public sealed class SecurityAttack() : Challenge("Too many suspicious visits!", Room.Control, points: 150, durationSeconds: 60)
{
    protected override bool IsRightAction(PlayerAction action)
    {
        return action == PlayerAction.EnableTrafficFilter;
    }
}
