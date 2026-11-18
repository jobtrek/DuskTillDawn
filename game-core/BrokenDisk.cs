namespace DuskTillDawn.Core;

/// <summary>A server disk failed. Fixed by swapping it in the server room.</summary>
public sealed class BrokenDisk() : Challenge("A disk is not responding", Room.Servers, points: 100, durationSeconds: 60)
{
    protected override bool IsRightAction(PlayerAction action)
    {
        return action == PlayerAction.ReplaceDisk;
    }
}
