namespace DuskTillDawn.Core;

// How many points a challenge is worth. Reading the dashboard early pays: a fast repair earns
// a bonus, and a missed challenge costs every player part of its points.
public static class PointsRule
{
    public const int FastRepairBonusPercent = 50;
    public const int FailurePenaltyPercent = 50;

    // A repair is fast while at least half of the challenge's time is left.
    public static bool IsFastRepair(double timeRemaining, double durationSeconds)
    {
        return timeRemaining >= durationSeconds / 2;
    }

    public static int RepairReward(int basePoints, double timeRemaining, double durationSeconds)
    {
        if (IsFastRepair(timeRemaining, durationSeconds))
        {
            return basePoints + basePoints * FastRepairBonusPercent / 100;
        }

        return basePoints;
    }

    public static int FailurePenalty(int basePoints)
    {
        return basePoints * FailurePenaltyPercent / 100;
    }
}
