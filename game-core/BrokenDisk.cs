namespace DuskTillDawn.Core;

/// <summary>A server disk failed. Fixed by swapping it in the server room.</summary>
public sealed class BrokenDisk : Challenge
{
    public BrokenDisk()
        : base("A disk is not responding", Room.Servers, points: 100, durationSeconds: 60)
    {
    }

    // The failed disk sits in one Server, picked at random in the room.
    public BrokenDisk(ServerRoom room)
        : this()
    {
        Hit(room.PickRandom());
    }

    protected override bool IsRightAction(PlayerAction action)
    {
        return action == PlayerAction.ReplaceDisk;
    }
}
