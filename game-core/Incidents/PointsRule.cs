namespace DuskTillDawn.Core.Incidents;

// How many points a incident is worth. Reading the dashboard early pays: a fast repair earns
// a bonus, and a missed incident costs every player part of its points.
public static class PointsRule
{
    public const int FastRepairBonusPercent = 50;
    public const int FailurePenaltyPercent = 50;

    // Surviving until 08:00 pays a fixed bonus plus one point for every SLA percent kept.
    public const int NightSurvivedBonus = 200;
    public const int PointsPerSlaPercent = 1;

    // A repair is fast while at least half of the incident's time is left.
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
