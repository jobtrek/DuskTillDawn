namespace DuskTillDawn.Core;

// A fire broke out. Fixed with the extinguisher in the server room.
public sealed class Fire() : Challenge("Fire alarm!", Room.Servers, points: 200, durationSeconds: 60)
{
    protected override bool IsRightAction(PlayerAction action)
    {
        return action == PlayerAction.UseExtinguisher;
    }
}
