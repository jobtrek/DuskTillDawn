using DuskTillDawn.Core.Incidents;
using DuskTillDawn.Core.Power;
using Godot;

// Shows and hides the Admin window, turns AdminApp.ActionPressed into a repair on the game,
// and drives the Power controls of the server room.
public partial class AdminController : Node
{
    private SoloGame _game = null!;
    private ServerRoom _serverRoom = null!;
    private AdminApp _app = null!;

    public override void _Ready()
    {
        // global:: because "Main" alone could resolve to the DuskTillDawn.Main namespace.
        var main = (global::Main)GetTree().CurrentScene;
        _game = main.Game;
        _serverRoom = main.State.ServerRoom;

        var menu = GetParent<ComputerMenu>();
        _app = menu.GetNode<AdminApp>("Map/Screen/AdminApp");
        _app.Hide();

        menu.AdminPressed += () =>
        {
            ShowPower();
            _app.Show();
        };
        // Only one app window at a time: the other apps sit in the same spot.
        menu.MailPressed += () => _app.Hide();
        menu.MonitoringPressed += () => _app.Hide();
        menu.VisibilityChanged += () =>
        {
            if (!menu.IsVisibleInTree())
            {
                _app.Hide();
            }
        };
        _app.CloseRequested += () => _app.Hide();
        _app.ActionPressed += OnActionPressed;
    }

    public override void _Process(double delta)
    {
        if (_app.IsVisibleInTree())
        {
            ShowPower();
        }
    }

    private void ShowPower()
    {
        string status = _serverRoom.Status switch
        {
            RoomPowerState.OnUps => $"Battery empty: UPS {_serverRoom.SecondsLeft:0} s left",
            RoomPowerState.Rebooting => $"Servers rebooting: {_serverRoom.SecondsLeft:0} s",
            RoomPowerState.ShutDownByHeat => "Servers off: too hot. Reboot them at the rack",
            RoomPowerState.ShutDownCleanly or RoomPowerState.Crashed => "Servers off: waiting for power",
            _ => $"Battery {_serverRoom.Battery:0} %",
        };
        _app.ShowPower(status, _serverRoom.EcoMode, _serverRoom.CanTurnOnEcoMode, _serverRoom.Status == RoomPowerState.OnUps);
    }

    // With no incident running, an admin action does nothing and costs nothing.
    private void OnActionPressed(string system, string action)
    {
        if (action == "EcoMode")
        {
            if (_serverRoom.EcoMode)
            {
                _serverRoom.TurnOffEcoMode();
            }
            else
            {
                _serverRoom.TryTurnOnEcoMode();
            }

            return;
        }

        if (action == "GracefulShutdown")
        {
            _serverRoom.TryGracefulShutdown();
            return;
        }

        if (!System.Enum.TryParse(action, out PlayerAction playerAction)
            || !System.Enum.IsDefined(playerAction))
        {
            GD.PushError($"Admin action '{system}/{action}' is not a PlayerAction.");
            return;
        }

        _game.Repair(_game.Players[0], playerAction);
    }
}
