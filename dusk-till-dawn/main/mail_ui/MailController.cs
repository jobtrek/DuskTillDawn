using Godot;
using DuskTillDawn.Core;


namespace DuskTillDawn.Main.MailUi;

/// <summary>
/// Glue between the desk computer, the mail window and the mail <see cref="Database"/>.
/// Lives as a child of ComputerMenu: opens the mail window on MailPressed and answers its signals.
/// </summary>
public partial class MailController : Node
{
    private readonly Database _db = new();
    private const int WrongReportPenalty = 25;

    private bool _seeded;
    private MailApp _app = null!;
    private GameState _state = null!;

    public override void _Ready()
    {
        // global:: because "Main" alone resolves to the DuskTillDawn.Main namespace here.
        _state = ((global::Main)GetTree().CurrentScene).State;

        var menu = GetParent<ComputerMenu>();
        _app = menu.GetNode<MailApp>("Map/Screen/MailApp");
        _app.Hide();

        menu.MailPressed += OpenMail;
        // Only one app window at a time: the monitoring window sits in the same spot.
        menu.MonitoringPressed += () => _app.Hide();
        // Player walked away from the computer: close the mail window too.
        menu.VisibilityChanged += () =>
        {
            if (!menu.IsVisibleInTree())
            {
                _app.Hide();
            }
        };

        _app.MailOpened += id =>
        {
            _db.MarkRead(id);
            Refresh();
        };
        _app.ReportPressed += id =>
        {
            bool correct = _db.ReportPhishing(id);
            if (!correct)
            {
                _state.LosePoints(WrongReportPenalty);
            }
            _app.ShowReportResult(id, correct);
            Refresh();
        };
        _app.CloseRequested += () => _app.Hide();
    }

    private void OpenMail()
    {
        if (!_seeded)
        {
            _db.GenerateDatabase();
            _seeded = true;
        }
        Refresh();
        _app.Show();
    }

    private void Refresh()
    {
        _app.SetMails(_db.GetAllEmails());
    }
}
