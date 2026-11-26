namespace DuskTillDawn.Core;

/// <summary>A server disk failed. Fixed by swapping it in the server room.</summary>
public sealed class BrokenDisk : Challenge
{
    public BrokenDisk()
        : base("A disk is not responding", Room.Servers, points: 100, durationSeconds: 60)
    {
    }

    // The failed disk sits in one Server, picked at random in the rack.
    public BrokenDisk(ServerRack rack)
        : this()
    {
        Hit(rack.PickRandom());
    }

    public override WarningText Warning
    {
        get
        {
            string server = TargetNames(whenNone: "A server");
            return new("Monitoring", $"Disk failing on {TargetNames(whenNone: "a server")}",
                $"{server} reports disk errors. Replace the drive before it fails.");
        }
    }

    protected override bool IsRightAction(PlayerAction action)
    {
        return action == PlayerAction.ReplaceDisk;
    }
}
