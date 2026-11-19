using DuskTillDawn.Core;
using Godot;

// Shows and hides the Admin window, and turns AdminApp.ActionPressed into a repair on the game.
public partial class AdminController : Node
{
    private SoloGame _game = null!;
    private AdminApp _app = null!;

    public override void _Ready()
    {
        // global:: because "Main" alone could resolve to the DuskTillDawn.Main namespace.
        _game = ((global::Main)GetTree().CurrentScene).Game;

        var menu = GetParent<ComputerMenu>();
        _app = menu.GetNode<AdminApp>("Map/Screen/AdminApp");
        _app.Hide();

        menu.AdminPressed += () => _app.Show();
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

    // With no challenge running, an admin action does nothing and costs nothing.
    private void OnActionPressed(string system, string action)
    {
        if (!System.Enum.TryParse(action, out PlayerAction playerAction)
            || !System.Enum.IsDefined(playerAction))
        {
            GD.PushError($"Admin action '{system}/{action}' is not a PlayerAction.");
            return;
        }

        _game.Repair(_game.Players[0], playerAction);
    }
}
