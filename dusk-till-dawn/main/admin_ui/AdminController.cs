using DuskTillDawn.Core;
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
            ShowBusy();
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
            ShowBusy();
        }
    }

    private void ShowBusy()
    {
        _app.ShowBusy(_game.Players[0].BusySecondsLeft);
    }

    private void ShowPower()
    {
        string status = _serverRoom.Status switch
        {
            ServerStatus.OnUps => $"Battery empty: UPS {_serverRoom.SecondsLeft:0} s left",
            ServerStatus.Rebooting => $"Servers rebooting: {_serverRoom.SecondsLeft:0} s",
            // A symptom, never the fix (docs/player-flow/challenges.md, Warning mails); "Servers off" matches the dashboard.
            ServerStatus.ShutDownByHeat when _serverRoom.Heat >= ServerRoom.ThrottleHeat => "Servers off: too hot",
            ServerStatus.ShutDownByHeat => "Servers off",
            ServerStatus.ShutDownCleanly or ServerStatus.Crashed => "Servers off: waiting for power",
            _ => $"Battery {_serverRoom.Battery:0} %",
        };
        _app.ShowPower(status, _serverRoom.EcoMode, _serverRoom.CanTurnOnEcoMode, _serverRoom.Status == ServerStatus.OnUps);
    }

    // With no challenge running, an admin action does nothing and costs nothing; a wrong one during a challenge
    // makes the player busy (Game.Repair decides).
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
