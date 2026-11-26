using System;
using System.Collections.Generic;
using System.Linq;
using DuskTillDawn.Core.Night;
using DuskTillDawn.Core.Servers;

namespace DuskTillDawn.Core.Incidents;

/// <summary>A game holds its players and incidents, moves them every tick and says when it is over.</summary>
public abstract class Game
{
    private readonly List<PlayerProfile> _players;
    private readonly List<Incident> _incidents = [];

    // Placeholder until the team agrees on the number (issue #105).
    private const double SlaDrainPerClient = 0.05;

    protected Game(IEnumerable<PlayerProfile> players)
    {
        _players = [.. players];
    }

    public IReadOnlyList<PlayerProfile> Players => _players;
    public IReadOnlyList<Incident> Incidents => _incidents;
    public int Failures { get; private set; }
    public int Solved { get; private set; }

    public event Action<Incident>? IncidentSucceeded;
    public event Action<Incident>? IncidentFailed;
    public event Action<Incident>? IncidentLaunched;



    public double SlaDrainPerMinute
    {
        get
        {
            double drain = 0;
            var broken = new HashSet<Server>();
            for (var i = 0; i < _incidents.Count; i++)
            {
                if (_incidents[i].State != IncidentState.Running)
                {
                    continue;
                }

                if (_incidents[i].Targets.Count == 0)
                {
                    drain += _incidents[i].SlaDrainPerMinute;
                }
                else
                {
                    broken.UnionWith(_incidents[i].Targets);
                }
            }

            drain += broken.SelectMany(server => server.Clients).Distinct().Count() * SlaDrainPerClient;

            return drain;
        }
    }

    public bool IsRunning<T>() where T : Incident
    {
        for (var i = 0; i < _incidents.Count; i++)
        {
            if (_incidents[i] is T && _incidents[i].State == IncidentState.Running)
            {
                return true;
            }
        }

        return false;
    }

    public abstract bool IsLost { get; }
    public abstract bool IsFinished { get; }

    /// <summary>Adds a incident that waits until it is launched.</summary>
    public void Add(Incident incident)
    {
        if (!_incidents.Contains(incident))
        {
            _incidents.Add(incident);
            incident.Id = _incidents.Count;
            incident.Succeeded += c =>
            {
                Solved++;
                IncidentSucceeded?.Invoke(c);
            };
        }
    }

    /// <summary>Adds a incident if needed and starts it.</summary>
    public void Launch(Incident incident)
    {
        Add(incident);
        bool wasPending = incident.State == IncidentState.Pending;
        incident.Start();
        RefreshServerStatus();

        if (wasPending)
        {
            IncidentLaunched?.Invoke(incident);
        }
    }

    // A player's fix reaches every incident; each one decides whether this action repairs it.
    public void Repair(PlayerProfile player, PlayerAction action)
    {
        foreach (Incident incident in _incidents)
        {
            incident.Repair(player, action);
        }

        RefreshServerStatus();
    }

    // Some tools only handle one problem per use (one extinguisher, one fire): the oldest running one is fixed.
    public bool RepairOne(PlayerProfile player, PlayerAction action)
    {
        for (var i = 0; i < _incidents.Count; i++)
        {
            if (_incidents[i].Repair(player, action))
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
        for (var i = 0; i < _incidents.Count; i++)
        {
            Incident incident = _incidents[i];
            bool wasRunning = incident.State == IncidentState.Running;

            incident.Tick(seconds);

            if (wasRunning && incident.State == IncidentState.Failed)
            {
                Failures++;
                LoseFailurePenalty(incident);
                IncidentFailed?.Invoke(incident);
            }
        }

        RefreshServerStatus();
    }

    // A Server is Down while a running incident hits it, and back to Ok once that one is fixed or failed.
    private void RefreshServerStatus()
    {
        for (var i = 0; i < _incidents.Count; i++)
        {
            foreach (Server server in _incidents[i].Targets)
            {
                server.Status = ServerHealth.Ok;
            }
        }

        for (var i = 0; i < _incidents.Count; i++)
        {
            if (_incidents[i].State != IncidentState.Running)
            {
                continue;
            }

            foreach (Server server in _incidents[i].Targets)
            {
                server.Status = ServerHealth.Down;
            }
        }
    }

    // A missed incident hurts the whole company, so every player pays for it.
    private void LoseFailurePenalty(Incident incident)
    {
        int penalty = PointsRule.FailurePenalty(incident.Points);
        for (var i = 0; i < _players.Count; i++)
        {
            _players[i].LosePoints(penalty);
        }
    }
}
