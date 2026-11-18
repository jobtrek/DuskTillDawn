using System;

namespace DuskTillDawn.Core;

/// <summary>
/// Two players race on the same challenges. The game ends once every challenge is succeeded or failed,
/// and the player with the most points wins.
/// </summary>
public sealed class DuoGame : Game
{
    public DuoGame(PlayerProfile first, PlayerProfile second, int challengeCount)
        : base([first, second])
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(challengeCount);
        ChallengeCount = challengeCount;
    }

    public int ChallengeCount { get; }

    /// <summary>Nobody loses the company in a race: there is only a winner.</summary>
    public override bool IsLost => false;

    public override bool IsFinished => FinishedChallenges() >= ChallengeCount;

    /// <summary>The player with the most points once the game is over, or null on a tie or before the end.</summary>
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
