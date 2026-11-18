using System.Linq;
using DuskTillDawn.Core;
using Godot;

namespace DuskTillDawn.Main.MailUi;

/// <summary>
/// Glue between the desk computer, the mail window and the <see cref="Inbox"/> in game-core.
/// Lives as a child of ComputerMenu: opens the mail window on MailPressed and answers its signals.
/// </summary>
public partial class MailController : Node
{
    private MailApp _app = null!;
    private GameState _state = null!;
    private Hud _hud = null!;

    public override void _Ready()
    {
        // global:: because "Main" alone resolves to the DuskTillDawn.Main namespace here.
        var main = (global::Main)GetTree().CurrentScene;
        _state = main.State;
        _hud = main.GetNode<Hud>("Hud");

        var menu = GetParent<ComputerMenu>();
        _app = menu.GetNode<MailApp>("Map/Screen/MailApp");
        _app.Hide();

        menu.MailPressed += OpenMail;
        // Only one app window at a time: the monitoring window sits in the same spot.
        menu.MonitoringPressed += () => _app.Hide();
        menu.VisibilityChanged += () =>
        {
            bool atDesk = menu.IsVisibleInTree();
            // At the desk the inbox itself shows the new mail; the toast would sit on top of the screen.
            _hud.MailToastMuted = atDesk;
            if (atDesk)
            {
                _hud.HideMailToast();
            }
            else
            {
                _app.Hide();
            }
        };

        _app.MailOpened += id =>
        {
            _state.Inbox.MarkRead(id);
            Refresh();
        };
        _app.ReportPressed += id =>
        {
            _app.ShowReportResult(id, _state.ReportPhishing(id));
            Refresh();
        };
        _app.CloseRequested += () => _app.Hide();

        _state.MailReceived += OnMailReceived;
    }

    public override void _ExitTree()
    {
        _state.MailReceived -= OnMailReceived;
    }

    private void OnMailReceived(Mail mail)
    {
        Refresh();
    }

    private void OpenMail()
    {
        Refresh();
        _app.Show();
    }

    private void Refresh()
    {
        var inbox = _state.Inbox;
        _app.SetMails(inbox.Mails.Reverse());
        _app.SetCount(inbox.Mails.Count, inbox.UnreadCount);
    }
}
