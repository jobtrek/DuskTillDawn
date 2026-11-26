using System;
using System.Linq;

namespace DuskTillDawn.Core.Tests;

public class SolarArrayTests
{
    [Fact]
    public void The_roof_has_nine_panels_with_different_names()
    {
        var panels = new SolarArray(new Random(1));

        Assert.Equal(9, panels.All.Count);
        Assert.Equal(9, panels.All.Select(panel => panel.Name).Distinct().Count());
    }

    [Fact]
    public void Picking_never_returns_a_panel_that_is_already_broken()
    {
        var panels = new SolarArray(new Random(1));
        var broken = panels.All.Take(8).ToList();

        for (var i = 0; i < 50; i++)
        {
            Assert.Equal(panels.All[8], panels.PickRandomExcept(broken));
        }
    }

    [Fact]
    public void Picking_returns_nothing_when_every_panel_is_broken()
    {
        var panels = new SolarArray(new Random(1));

        Assert.Null(panels.PickRandomExcept(panels.All));
    }
}
