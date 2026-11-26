namespace DuskTillDawn.Core;

public sealed class DamagedSolarPanel(SolarPanel? target = null) : Challenge("A solar panel is damaged", Room.Roof, points: 100, durationSeconds: 60)
{
    public SolarPanel? Target { get; } = target;
    public override WarningText Warning => new("Facility", $"The {Target?.Name ?? "solar panel"} is damaged", "The solar panel is not producing power. Repair it before the battery runs out.");

    protected override bool IsRightAction(PlayerAction action)
    {
        return action == PlayerAction.RepairPanel;
    }
}
