using System.Collections.Generic;
using DuskTillDawn.Core.Building;
using DuskTillDawn.Core.Incidents;
using Godot;

/// <summary>
/// Small HUD panel (top right): points, failures out of 3 and every running incident with its room and timer.
/// Main calls ShowGame every frame while the dashboard is visible. The dashboard only reads the game; it never changes it.
/// </summary>
public partial class Dashboard : CanvasLayer
{
    private const double UrgentSeconds = 15;

    [Export] public float AppearDuration = 0.4f;
    [Export] public float PopDuration = 0.25f;

    private static readonly Color Faded = new(1f, 1f, 1f, 0.55f);

    private Control _panel = null!;
    private Label _pointsValue = null!;
    private Label _failuresValue = null!;
    private Label _emptyLabel = null!;
    private VBoxContainer _alertList = null!;

    private readonly List<AlertRow> _rows = new();
    private int? _points;
    private int _failures = -1;
    private bool? _lost;
    private readonly Dictionary<Control, Tween> _popTweens = new();

    public override void _Ready()
    {
        _panel = GetNode<Control>("%Panel");
        _pointsValue = GetNode<Label>("%PointsValue");
        _failuresValue = GetNode<Label>("%FailuresValue");
        _emptyLabel = GetNode<Label>("%EmptyLabel");
        _alertList = GetNode<VBoxContainer>("%AlertList");
        PlayAppear();
    }

    public void ShowGame(SoloGame game)
    {
        ShowPoints(game.Players[0].Points);
        ShowFailures(game.Failures, game.IsLost);
        ShowAlerts(game.Incidents);
    }

    private void ShowPoints(int points)
    {
        if (points == _points)
        {
            return;
        }

        int? previous = _points;
        _points = points;
        _pointsValue.Text = points.ToString();
        if (previous != null)
        {
            UiMotion.Pop(_pointsValue, points < previous ? UiMotion.LossTint : UiMotion.GainTint, PopDuration, _popTweens);
        }
    }

    private void ShowFailures(int failures, bool lost)
    {
        if (failures == _failures && lost == _lost)
        {
            return;
        }

        bool firstShow = _failures < 0;
        bool increased = failures > _failures;
        _failures = failures;
        _lost = lost;
        _failuresValue.Text = lost ? $"{failures} / {SoloGame.MaxFailures}  GAME OVER" : $"{failures} / {SoloGame.MaxFailures}";
        _failuresValue.SelfModulate = lost ? UiMotion.LossTint : Colors.White;

        if (!firstShow && increased)
        {
            UiMotion.Pop(_failuresValue, UiMotion.LossTint, PopDuration, _popTweens);
        }
    }

    private void ShowAlerts(IReadOnlyList<Incident> incidents)
    {
        int shown = 0;
        for (int i = 0; i < incidents.Count; i++)
        {
            Incident incident = incidents[i];
            if (incident.State != IncidentState.Running)
            {
                continue;
            }

            if (shown == _rows.Count)
            {
                _rows.Add(new AlertRow(_alertList));
            }

            _rows[shown].Show(incident);
            shown++;
        }

        for (int i = shown; i < _rows.Count; i++)
        {
            _rows[i].Hide();
        }

        _emptyLabel.Visible = shown == 0;
    }

    // The panel slides in from the right when the game starts.
    private void PlayAppear()
    {
        UiMotion.SlideIn(_panel, new Vector2(80f, 0f), AppearDuration);
    }

    private static string RoomName(Room room)
    {
        return room switch
        {
            Room.Roof => "Roof",
            Room.Control => "Control room",
            _ => "Server room",
        };
    }

    /// <summary>One line of the alert list. Texts change only when the shown value changes.</summary>
    private sealed class AlertRow
    {
        private readonly HBoxContainer _box;
        private readonly Label _alert;
        private readonly Label _room;
        private readonly Label _time;
        private Incident? _incident;
        private int _seconds = -1;

        public AlertRow(VBoxContainer parent)
        {
            _box = new HBoxContainer { MouseFilter = Control.MouseFilterEnum.Ignore };
            _box.AddThemeConstantOverride("separation", 8);
            _alert = new Label { SizeFlagsHorizontal = Control.SizeFlags.ExpandFill, ClipText = true };
            _alert.AddThemeFontSizeOverride("font_size", 14);
            _room = new Label { SelfModulate = Faded };
            _time = new Label { CustomMinimumSize = new Vector2(40, 0), HorizontalAlignment = HorizontalAlignment.Right };
            _time.AddThemeFontSizeOverride("font_size", 14);
            _box.AddChild(_alert);
            _box.AddChild(_room);
            _box.AddChild(_time);
            parent.AddChild(_box);
        }

        public void Show(Incident incident)
        {
            _box.Visible = true;
            if (incident != _incident)
            {
                _incident = incident;
                _seconds = -1;
                _alert.Text = incident.Alert;
                _room.Text = RoomName(incident.Room);
            }

            int seconds = Mathf.CeilToInt(incident.TimeRemaining);
            if (seconds == _seconds)
            {
                return;
            }

            _seconds = seconds;
            _time.Text = $"{seconds / 60}:{seconds % 60:00}";
            _time.SelfModulate = seconds <= UrgentSeconds ? UiMotion.LossTint : Colors.White;
        }

        public void Hide()
        {
            _box.Visible = false;
            _incident = null;
        }
    }
}
