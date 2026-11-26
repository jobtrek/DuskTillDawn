using System.Linq;
using DuskTillDawn.Core.Mails;
using DuskTillDawn.Core.Night;
using Godot;

namespace DuskTillDawn.Main.MailUi;

public partial class MailController : Node
{
    private MailApp _app = null!;
    private GameState _state = null!;
    private PhishingIncidents _phishing = null!;
    private Hud _hud = null!;
    private ComputerMenu _menu = null!;

    public override void _Ready()
    {
        // global:: because "Main" alone resolves to the DuskTillDawn.Main namespace here.
        var main = (global::Main)GetTree().CurrentScene;
        _state = main.State;
        _phishing = main.Phishing;
        _hud = main.GetNode<Hud>("Hud");

        var menu = GetParent<ComputerMenu>();
        _menu = menu;
        _app = menu.GetNode<MailApp>("Map/Screen/MailApp");
        _app.Hide();

        menu.MailPressed += OpenMail;
        // Only one app window at a time: the monitoring window sits in the same spot.
        menu.MonitoringPressed += () => _app.Hide();
        menu.AdminPressed += () => _app.Hide();
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
            PhishingReport report = _phishing.Report(id);
            _app.ShowReportResult(id, report.IsPhishing, report.Points);
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
        _menu.SetMailUnread(inbox.UnreadCount);
    }
}
