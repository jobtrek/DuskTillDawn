namespace DuskTillDawn.Core;

public enum ChallengeState
{
    Pending,
    Running,
    Succeeded,
    Failed,
}

public enum Room
{
    Roof,
    Control,
    Servers,
}

public abstract class Challenge
{
    private const double RoundingMargin = 1e-9;

    protected Challenge(string alert, Room room, int points, double durationSeconds)
    {
        Alert = alert;
        Room = room;
        Points = points;
        DurationSeconds = durationSeconds;
        TimeRemaining = durationSeconds;
    }



    // getters and setters allow other classes to modify private properties. 
    public string Alert { get; }
    public Room Room { get; }
    public int Points { get; }
    public double DurationSeconds { get; }
    public double TimeRemaining { get; private set; }
    public ChallengeState State { get; private set; } = ChallengeState.Pending;

    public void Start()
    {
        if (State == ChallengeState.Pending)
        {
            State = ChallengeState.Running;
        }
    }

    public void Tick(double seconds)
    {
        if (State != ChallengeState.Running)
        {
            return;
        }

        TimeRemaining -= seconds;

        if (TimeRemaining <= RoundingMargin)
        {
            TimeRemaining = 0;
            State = ChallengeState.Failed;
        }
    }

    public void Repair(PlayerProfile player)
    {
        if (State != ChallengeState.Running)
        {
            return;
        }
        State = ChallengeState.Succeeded;
        player.EarnPoints(Points);
    }
}
