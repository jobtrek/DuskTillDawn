using DuskTillDawn.Core.Building;
using DuskTillDawn.Core.Mails;

namespace DuskTillDawn.Core.Incidents;

// A fire broke out. Fixed with the extinguisher in the server room.
public sealed class Fire() : Incident("Fire alarm!", Room.Servers, points: 200, durationSeconds: 60)
{
    public override WarningText Warning => new("Facility", "Smoke detected in the server room",
        "Fire alarm triggered. Grab an extinguisher.");

    protected override bool IsRightAction(PlayerAction action)
    {
        return action == PlayerAction.UseExtinguisher;
    }
}
