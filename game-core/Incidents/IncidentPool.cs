using System;
using System.Collections.Generic;

namespace DuskTillDawn.Core.Incidents;

public sealed class IncidentPool
{
    private readonly List<Func<Incident>> _incidents;
    private readonly Random _random;

    public IncidentPool(IEnumerable<Func<Incident>> incidents, Random? random = null)
    {
        _incidents = [.. incidents];
        if (_incidents.Count == 0)
        {
            throw new ArgumentException("Give at least one incident to store.", nameof(incidents));
        }

        _random = random ?? new Random();
    }

    public Incident PickRandom()
    {
        return _incidents[_random.Next(_incidents.Count)]();
    }
}
