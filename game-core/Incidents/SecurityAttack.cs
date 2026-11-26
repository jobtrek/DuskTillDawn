using DuskTillDawn.Core.Building;
using DuskTillDawn.Core.Mails;

namespace DuskTillDawn.Core.Incidents;

/// <summary>Too much suspicious traffic. Fixed from the control room computer.</summary>
public sealed class SecurityAttack() : Incident("Too many suspicious visits!", Room.Control, points: 150, durationSeconds: 60)
{
    public override WarningText Warning => new("IT Security", "Suspicious traffic spike",
        "Someone is flooding the network. Turn on the traffic filter.");

    protected override bool IsRightAction(PlayerAction action)
    {
        return action == PlayerAction.EnableTrafficFilter;
    }
}
