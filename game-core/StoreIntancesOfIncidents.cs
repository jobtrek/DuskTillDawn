using System;
using System.Collections.Generic;

namespace DuskTillDawn.Core;

public sealed class StoreInstancesOfIncidents
{
    private readonly List<Func<Challenge>> _incidents;
    private readonly Random _random;

    public StoreInstancesOfIncidents(IEnumerable<Func<Challenge>> incidents, Random? random = null)
    {
        _incidents = [.. incidents];
        if (_incidents.Count == 0)
        {
            throw new ArgumentException("Give at least one incident to store.", nameof(incidents));
        }

        _random = random ?? new Random();
    }

    public Challenge PickRandom()
    {
        return _incidents[_random.Next(_incidents.Count)]();
    }
}
