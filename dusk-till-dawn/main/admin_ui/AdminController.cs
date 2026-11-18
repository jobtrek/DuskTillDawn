using Godot;

// Shows and hides the Admin window. The fix itself is decided by Main, which listens to AdminApp.ActionPressed.
public partial class AdminController : Node
{
    private AdminApp _app = null!;

    public override void _Ready()
    {
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
    }
}
