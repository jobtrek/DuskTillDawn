using DuskTillDawn.Core.Building;
using DuskTillDawn.Core.Mails;

namespace DuskTillDawn.Core.Incidents;

// A fault in the basement power equipment. Left alone it starts a Fire (see ElectricalFaultRisk).
public sealed class ElectricalFault() : Incident("Power fault in the basement", Room.Servers, points: 150, durationSeconds: 60)
{
    public override WarningText Warning => new("Facility", "Electrical fault",
        "A circuit is shorting. Cut it at the panel.");

    protected override bool IsRightAction(PlayerAction action)
    {
        return action == PlayerAction.CutFaultyCircuit;
    }
}
