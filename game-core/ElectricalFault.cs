namespace DuskTillDawn.Core;

// A fault in the basement power equipment. Left alone it starts a Fire (see ElectricalFaultRisk).
public sealed class ElectricalFault() : Challenge("Power fault in the basement", Room.Servers, points: 150, durationSeconds: 60)
{
    protected override bool IsRightAction(PlayerAction action)
    {
        return action == PlayerAction.CutFaultyCircuit;
    }
}
