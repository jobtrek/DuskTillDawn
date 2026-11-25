using Godot;
using DuskTillDawn.Core;

/// <summary>
/// Glue between the desk computer, the GameState values and the monitoring window.
/// Lives as a child of ComputerMenu: opens the window on MonitoringPressed and keeps it updated.
/// </summary>
public partial class MonitoringController : Node
{
    private const double Empty = 0;
    private const double Full = 100;

    private GameState _state = null!;
    private MonitorApp _app = null!;

    public override void _Ready()
    {
        // global:: because "Main" alone could resolve to the DuskTillDawn.Main namespace.
        _state = ((global::Main)GetTree().CurrentScene).State;

        var menu = GetParent<ComputerMenu>();
        _app = menu.GetNode<MonitorApp>("Map/Screen/MonitorApp");
        _app.Hide();

        menu.MonitoringPressed += OpenMonitor;
        // Only one app window at a time: the mail window sits in the same spot.
        menu.MailPressed += () => _app.Hide();
        menu.AdminPressed += () => _app.Hide();
        // Player walked away from the computer: close the window too.
        menu.VisibilityChanged += () =>
        {
            if (!menu.IsVisibleInTree())
            {
                _app.Hide();
            }
        };
        _app.CloseRequested += () => _app.Hide();
    }

    public override void _Process(double delta)
    {
        if (_app.IsVisibleInTree())
        {
            Refresh();
        }
    }

    private void OpenMonitor()
    {
        Refresh();
        _app.Show();
    }

    private void Refresh()
    {
        _app.SetMetric(Metric.Power, _state.Power, MonitorDanger.Danger(Full, Empty, _state.Power));
        _app.SetMetric(Metric.Heat, _state.Heat, MonitorDanger.Danger(ServerRoom.MinHeat, ServerRoom.ShutdownHeat, _state.Heat));
        _app.SetMetric(Metric.Traffic, _state.Traffic, MonitorDanger.Danger(ServerRoom.NormalTraffic, Full, _state.Traffic));
        _app.SetMetric(Metric.Disk, _state.Disk, MonitorDanger.Danger(Empty, Full, _state.Disk));
        _app.SetMetric(Metric.Sla, _state.Sla, MonitorDanger.Danger(Full, Empty, _state.Sla));
        _app.SetServers(_state.Servers.All);
    }
}
