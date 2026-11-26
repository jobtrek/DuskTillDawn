using System;
using System.Collections.Generic;
using System.Linq;

namespace DuskTillDawn.Core;

public sealed class SolarArray
{
    private readonly List<SolarPanel> _panels = [];
    private readonly Random _random;

    public SolarArray(Random? random = null)
    {
        _random = random ?? new Random();
        for (int i = 1; i <= 9; i++)
        {
            _panels.Add(new SolarPanel($"PANEL-{i:00}"));
        }
    }

    public IReadOnlyList<SolarPanel> All => _panels;

    public SolarPanel PickRandom()
    {
        return _panels[_random.Next(_panels.Count)];
    }

    public SolarPanel? PickRandomExcept(IEnumerable<SolarPanel> broken)
    {
        var list = _panels.FindAll(p => !broken.Contains(p));
        if (list.Count == 0)
        {
            return null;
        }

        return list[_random.Next(list.Count)];
    }
}
