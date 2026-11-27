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

    // Placeholder until the team validates the number (issue #136).
    public const double WrongActionBusySeconds = 5;

    protected Game(IEnumerable<PlayerProfile> players)
    {
        _players = [.. players];
    }

    public IReadOnlyList<PlayerProfile> Players => _players;
    public IReadOnlyList<Challenge> Challenges => _challenges;
    public int Failures { get; private set; }
    public int Solved { get; private set; }

    public event Action<Challenge>? ChallengeSucceeded;
    public event Action<Challenge>? ChallengeFailed;
    public event Action<Challenge>? ChallengeLaunched;



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

            drain += broken.SelectMany(server => server.Clients).Distinct().Count() * SlaDrainPerClient;

            return drain;
        }
    }

    public bool IsRunning<T>() where T : Challenge
    {
        for (var i = 0; i < _challenges.Count; i++)
        {
            if (_challenges[i] is T && _challenges[i].State == ChallengeState.Running)
            {
                return true;
            }
        }

        return false;
    }

    public abstract bool IsLost { get; }
    public abstract bool IsFinished { get; }

    /// <summary>Adds a challenge that waits until it is launched.</summary>
    public void Add(Challenge challenge)
    {
        if (!_challenges.Contains(challenge))
        {
            _challenges.Add(challenge);
            challenge.Id = _challenges.Count;
            challenge.Succeeded += c =>
            {
                Solved++;
                ChallengeSucceeded?.Invoke(c);
            };
        }
    }

    /// <summary>Adds a challenge if needed and starts it.</summary>
    public void Launch(Challenge challenge)
    {
        Add(challenge);
        bool wasPending = challenge.State == ChallengeState.Pending;
        challenge.Start();
        RefreshServerStatus();

        if (wasPending)
        {
            ChallengeLaunched?.Invoke(challenge);
        }
    }

    // A player's fix reaches every challenge; each one decides whether this action repairs it.
    // Guessing has a price: an action that fixes nothing while something is broken wastes the player's time.
    public bool Repair(PlayerProfile player, PlayerAction action)
    {
        if (player.IsBusy)
        {
            return false;
        }

        bool somethingWasBroken = IsRunning<Challenge>();
        bool fixedSomething = false;
        foreach (Challenge challenge in _challenges)
        {
            if (challenge.Repair(player, action))
            {
                fixedSomething = true;
            }
        }

        if (!fixedSomething && somethingWasBroken)
        {
            player.MakeBusy(WrongActionBusySeconds);
        }

        RefreshServerStatus();
        return fixedSomething;
    }

    // Some tools only handle one problem per use (one extinguisher, one fire): the oldest running one is fixed.
    public bool RepairOne(PlayerProfile player, PlayerAction action)
    {
        if (player.IsBusy)
        {
            return false;
        }

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

    public bool RepairPanel(PlayerProfile player, SolarPanel panel)
    {
        if (player.IsBusy)
        {
            return false;
        }

        for (var i = 0; i < _challenges.Count; i++)
        {
            if (_challenges[i] is DamagedSolarPanel damaged && damaged.Target == panel)
            {
                if (damaged.Repair(player, PlayerAction.RepairPanel))
                {
                    RefreshServerStatus();
                    return true;
                }
            }
        }

        return false;
    }

    // Some fixes only count once the room is back to normal (see ChallengeState.Recovering); Main calls this after each room tick.
    public void FinishRecovered(ServerRoom room)
    {
        // Like Tick: once the game is lost, nothing moves and no more points are paid.
        if (IsFinished)
        {
            return;
        }

        bool finishedAny = false;
        for (var i = 0; i < _challenges.Count; i++)
        {
            if (_challenges[i].TryFinishRecovery(room))
            {
                finishedAny = true;
            }
        }

        if (finishedAny)
        {
            RefreshServerStatus();
        }
    }

    public void Tick(double seconds)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(seconds);

        if (IsFinished)
        {
            return;
        }

        // A for loop over the list keeps the tick free of allocations.
        for (var i = 0; i < _players.Count; i++)
        {
            _players[i].Tick(seconds);
        }

        for (var i = 0; i < _challenges.Count; i++)
        {
            Challenge challenge = _challenges[i];
            bool wasRunning = challenge.State == ChallengeState.Running;

            challenge.Tick(seconds);

            if (wasRunning && challenge.State == ChallengeState.Failed)
            {
                Failures++;
                LoseFailurePenalty(challenge);
                ChallengeFailed?.Invoke(challenge);
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
                server.Status = ServerHealth.Ok;
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
                server.Status = ServerHealth.Down;
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
