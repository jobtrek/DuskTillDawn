namespace DuskTillDawn.Core;

// How many points a challenge is worth. Reading the dashboard early pays: a fast repair earns
// a bonus, and a missed challenge costs every player part of its points.
public static class PointsRule
{
    public const int FastRepairBonusPercent = 50;
    public const int FailurePenaltyPercent = 50;

    // Surviving until 08:00 pays a fixed bonus plus one point for every SLA percent kept.
    public const int NightSurvivedBonus = 200;
    public const int PointsPerSlaPercent = 1;

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

    public static int NightBonus(double slaLeft)
    {
        return NightSurvivedBonus + (int)slaLeft * PointsPerSlaPercent;
    }
}
