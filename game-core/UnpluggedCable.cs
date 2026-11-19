namespace DuskTillDawn.Core;

// A server cable came out. Fixed by plugging it back in the server room.
public sealed class UnpluggedCable() : Challenge("A server is offline", Room.Servers, points: 100, durationSeconds: 60)
{
    protected override bool IsRightAction(PlayerAction action)
    {
        return action == PlayerAction.PlugCable;
    }
}
