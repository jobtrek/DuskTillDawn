namespace DuskTillDawn.Core;

public sealed class DamagedSolarPanel() : Challenge("A solar panel is damaged", Room.Roof, points: 100, durationSeconds: 60)
{
    protected override bool IsRightAction(PlayerAction action)
    {
        return action == PlayerAction.RepairPanel;
    }
}
