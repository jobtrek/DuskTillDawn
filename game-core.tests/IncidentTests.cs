namespace DuskTillDawn.Core.Tests;

public class IncidentTests
{
    private static Incident NewIncident()
    {
        return new Incident("Failing hard drive", Room.Basement, points: 100);
    }

    [Fact]
    public void New_incident_is_dormant()
    {
        var incident = NewIncident();

        Assert.Equal(IncidentState.Dormant, incident.State);
        Assert.Equal("Failing hard drive", incident.Name);
        Assert.Equal(Room.Basement, incident.Room);
        Assert.Equal(100, incident.Points);
    }

    [Fact]
    public void Starting_an_incident_makes_it_a_warning()
    {
        var incident = NewIncident();

        incident.Start();
        incident.Start();

        Assert.Equal(IncidentState.Warning, incident.State);
    }

    [Fact]
    public void Dormant_incident_ignores_time()
    {
        var incident = NewIncident();

        incident.Tick(1000);

        Assert.Equal(IncidentState.Dormant, incident.State);
    }

    [Fact]
    public void Ignored_warning_becomes_active_after_60_s()
    {
        var incident = NewIncident();
        incident.Start();

        incident.Tick(59);
        Assert.Equal(IncidentState.Warning, incident.State);

        incident.Tick(1);
        Assert.Equal(IncidentState.Active, incident.State);
    }

    [Fact]
    public void Ignored_active_incident_escalates_after_3_min()
    {
        var incident = NewIncident();
        incident.Start();
        incident.Tick(60);

        incident.Tick(179);
        Assert.Equal(IncidentState.Active, incident.State);

        incident.Tick(1);
        Assert.Equal(IncidentState.Escalated, incident.State);
    }

    [Fact]
    public void One_big_tick_can_go_straight_to_escalated()
    {
        var incident = NewIncident();
        incident.Start();

        incident.Tick(240);

        Assert.Equal(IncidentState.Escalated, incident.State);
    }

    [Fact]
    public void Many_small_ticks_equal_one_big_tick()
    {
        var incident = NewIncident();
        incident.Start();

        for (var i = 0; i < 60 * 60; i++)
        {
            incident.Tick(1.0 / 60.0);
        }

        Assert.Equal(IncidentState.Active, incident.State);
    }

    [Fact]
    public void Fixing_during_warning_gives_50_percent_more_points()
    {
        var incident = NewIncident();
        incident.Start();

        Assert.Equal(150, incident.Resolve());
        Assert.Equal(IncidentState.Resolved, incident.State);
    }

    [Theory]
    [InlineData(60)]
    [InlineData(240)]
    public void Fixing_when_active_or_escalated_gives_base_points(double seconds)
    {
        var incident = NewIncident();
        incident.Start();
        incident.Tick(seconds);

        Assert.Equal(100, incident.Resolve());
        Assert.Equal(IncidentState.Resolved, incident.State);
    }

    [Fact]
    public void Dormant_or_resolved_incident_gives_nothing()
    {
        var incident = NewIncident();
        Assert.Equal(0, incident.Resolve());
        Assert.Equal(IncidentState.Dormant, incident.State);

        incident.Start();
        incident.Resolve();
        Assert.Equal(0, incident.Resolve());
    }

    [Fact]
    public void Resolved_incident_ignores_time()
    {
        var incident = NewIncident();
        incident.Start();
        incident.Resolve();

        incident.Tick(1000);

        Assert.Equal(IncidentState.Resolved, incident.State);
    }
}
