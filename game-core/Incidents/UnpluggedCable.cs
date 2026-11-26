using DuskTillDawn.Core.Building;
using DuskTillDawn.Core.Mails;

namespace DuskTillDawn.Core.Incidents;

// A server cable came out. Fixed by plugging it back in the server room.
public sealed class UnpluggedCable() : Incident("A server is offline", Room.Servers, points: 100, durationSeconds: 60)
{
    public override WarningText Warning => new("Monitoring", "Network link down",
        "A cable came loose. Plug it back in.");

    protected override bool IsRightAction(PlayerAction action)
    {
        return action == PlayerAction.PlugCable;
    }
}
