using System;
using System.Collections.Generic;
using System.Linq;

namespace DuskTillDawn.Core;

/// <summary>A game holds its players and challenges, moves them every tick and says when it is over.</summary>
public abstract class Game
{
    private readonly List<PlayerProfile> _players;
    private readonly List<Challenge> _challenges = [];

    // Placeholder until the team agrees on the number (issue #105).
    private const double SlaDrainPerClient = 0.05;

    protected Game(IEnumerable<PlayerProfile> players)
    {
        _players = [.. players];
    }

    public IReadOnlyList<PlayerProfile> Players => _players;
    public IReadOnlyList<Challenge> Challenges => _challenges;
    public int Failures { get; private set; }

    public event Action<Challenge>? ChallengeSucceeded;



    public double SlaDrainPerMinute
    {
        get
        {
            double drain = 0;
            var broken = new HashSet<Server>();
            for (var i = 0; i < _challenges.Count; i++)
            {
                if (_challenges[i].State != ChallengeState.Running)
                {
                    continue;
                }

                if (_challenges[i].Targets.Count == 0)
                {
                    drain += _challenges[i].SlaDrainPerMinute;
                }
                else
                {
                    broken.UnionWith(_challenges[i].Targets);
                }
            }

            foreach (Server server in broken)
            {
                drain += server.Clients * SlaDrainPerClient;
            }

            return drain;
        }
    }

    public abstract bool IsLost { get; }
    public abstract bool IsFinished { get; }

    /// <summary>Adds a challenge that waits until it is launched.</summary>
    public void Add(Challenge challenge)
    {
        if (!_challenges.Contains(challenge))
        {
            _challenges.Add(challenge);
            challenge.Succeeded += c => ChallengeSucceeded?.Invoke(c);
        }
    }

    /// <summary>Adds a challenge if needed and starts it.</summary>
    public void Launch(Challenge challenge)
    {
        Add(challenge);
        challenge.Start();
        RefreshServerStatus();
    }

    // A player's fix reaches every challenge; each one decides whether this action repairs it.
    public void Repair(PlayerProfile player, PlayerAction action)
    {
        foreach (Challenge challenge in _challenges)
        {
            challenge.Repair(player, action);
        }

        RefreshServerStatus();
    }

    // Some tools only handle one problem per use (one extinguisher, one fire): the oldest running one is fixed.
    public bool RepairOne(PlayerProfile player, PlayerAction action)
    {
        for (var i = 0; i < _challenges.Count; i++)
        {
            if (_challenges[i].Repair(player, action))
            {
                RefreshServerStatus();
                return true;
            }
        }

        return false;
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

        RefreshServerStatus();
    }

    // A Server is Down while a running challenge hits it, and back to Ok once that one is fixed or failed.
    private void RefreshServerStatus()
    {
        for (var i = 0; i < _challenges.Count; i++)
        {
            foreach (Server server in _challenges[i].Targets)
            {
                server.Status = ServerStatus.Ok;
            }
        }

        for (var i = 0; i < _challenges.Count; i++)
        {
            if (_challenges[i].State != ChallengeState.Running)
            {
                continue;
            }

            foreach (Server server in _challenges[i].Targets)
            {
                server.Status = ServerStatus.Down;
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
