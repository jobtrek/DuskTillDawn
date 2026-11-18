using System;

namespace DuskTillDawn.Core;

// Two players race on the same challenges; whoever has the most points after the last one wins.
public sealed class DuoGame : Game
{
    public DuoGame(PlayerProfile first, PlayerProfile second, int challengeCount)
        : base([first, second])
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(challengeCount);
        ChallengeCount = challengeCount;
    }

    public int ChallengeCount { get; }

    // A race has a winner, never a lost company.
    public override bool IsLost => false;

    public override bool IsFinished => FinishedChallenges() >= ChallengeCount;

    // A tie gives no winner, and nobody wins before the last challenge ends.
    public PlayerProfile? Winner
    {
        get
        {
            if (!IsFinished)
            {
                return null;
            }

            PlayerProfile first = Players[0];
            PlayerProfile second = Players[1];
            if (first.Points == second.Points)
            {
                return null;
            }

            return first.Points > second.Points ? first : second;
        }
    }

    // A for loop keeps this free of allocations, since the game asks for it every tick.
    private int FinishedChallenges()
    {
        int finished = 0;
        for (var i = 0; i < Challenges.Count; i++)
        {
            ChallengeState state = Challenges[i].State;
            if (state == ChallengeState.Succeeded || state == ChallengeState.Failed)
            {
                finished++;
            }
        }

        return finished;
    }
}
