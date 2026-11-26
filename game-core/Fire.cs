namespace DuskTillDawn.Core;

// A fire broke out. Fixed with the extinguisher in the server room.
public sealed class Fire(double durationSeconds = 60) : Challenge("Fire alarm!", Room.Servers, points: 200, durationSeconds)
{
    public override WarningText Warning => new("Facility", "Smoke detected in the server room",
        "Fire alarm triggered. Grab an extinguisher.");

    protected override bool IsRightAction(PlayerAction action)
    {
        return action == PlayerAction.UseExtinguisher;
    }
}
