namespace DuskTillDawn.Core;

// A server is too hot. Fixed by restarting the cooling in the server room.
public sealed class OverheatingServer() : Challenge("Temperature too high", Room.Servers, points: 100, durationSeconds: 60)
{
    protected override bool IsRightAction(PlayerAction action)
    {
        return action == PlayerAction.RestartCooling;
    }
}
