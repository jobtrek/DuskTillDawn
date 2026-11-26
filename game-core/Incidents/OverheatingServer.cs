using DuskTillDawn.Core.Building;
using DuskTillDawn.Core.Mails;

namespace DuskTillDawn.Core.Incidents;

// A server is too hot. Fixed by restarting the cooling in the server room.
public sealed class OverheatingServer() : Incident("Temperature too high", Room.Servers, points: 100, durationSeconds: 75)
{
    public override WarningText Warning => new("Monitoring", "Server room overheating",
        "Temperature is climbing fast. Restart the cooling.");

    protected override bool IsRightAction(PlayerAction action)
    {
        return action == PlayerAction.RestartCooling;
    }
}
