using System;
using System.Collections.Generic;

namespace DuskTillDawn.Core;

/// <summary>A game holds its players and challenges, moves them every tick and says when it is over.</summary>
public abstract class Game
{
    private readonly List<PlayerProfile> _players;
    private readonly List<Challenge> _challenges = [];

    protected Game(IEnumerable<PlayerProfile> players)
    {
        _players = [.. players];
    }

    public IReadOnlyList<PlayerProfile> Players => _players;
    public IReadOnlyList<Challenge> Challenges => _challenges;
    public int Failures { get; private set; }

    public abstract bool IsLost { get; }
    public abstract bool IsFinished { get; }

    /// <summary>Adds a challenge that waits until it is launched.</summary>
    public void Add(Challenge challenge)
    {
        if (!_challenges.Contains(challenge))
        {
            _challenges.Add(challenge);
        }
    }

    /// <summary>Adds a challenge if needed and starts it.</summary>
    public void Launch(Challenge challenge)
    {
        Add(challenge);
        challenge.Start();
    }

    public void Tick(double seconds)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(seconds);

        if (IsFinished)
        {
            return;
        }

        // A for loop over the list keeps the tick free of allocations.
        for (var i = 0; i < _challenges.Count; i++)
        {
            Challenge challenge = _challenges[i];
            bool wasRunning = challenge.State == ChallengeState.Running;

            challenge.Tick(seconds);

            if (wasRunning && challenge.State == ChallengeState.Failed)
            {
                Failures++;
                LoseFailurePenalty(challenge);
            }
        }
    }

    // A missed challenge hurts the whole company, so every player pays for it.
    private void LoseFailurePenalty(Challenge challenge)
    {
        int penalty = PointsRule.FailurePenalty(challenge.Points);
        for (var i = 0; i < _players.Count; i++)
        {
            _players[i].LosePoints(penalty);
        }
    }
}
