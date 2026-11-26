using DuskTillDawn.Core.Building;
using DuskTillDawn.Core.Mails;

namespace DuskTillDawn.Core.Incidents;

public sealed class DamagedSolarPanel() : Incident("A solar panel is damaged", Room.Roof, points: 100, durationSeconds: 60)
{
    public override WarningText Warning => new("Facility", "Solar panel damaged",
        "Lightning hit a panel on the roof. Go up and repair it.");

    protected override bool IsRightAction(PlayerAction action)
    {
        return action == PlayerAction.RepairPanel;
    }
}
