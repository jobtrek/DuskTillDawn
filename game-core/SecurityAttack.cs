namespace DuskTillDawn.Core;

/// <summary>Too much suspicious traffic. Fixed from the control room computer.</summary>
public sealed class SecurityAttack(double durationSeconds = 60)
    : Challenge("Too many suspicious visits!", Room.Control, points: 150, durationSeconds)
{
    public override WarningText Warning => new("IT Security", "Suspicious traffic spike",
        "Someone is flooding the network. Check the traffic on Monitoring.");

    protected override bool IsRightAction(PlayerAction action)
    {
        return action == PlayerAction.EnableTrafficFilter;
    }
}
