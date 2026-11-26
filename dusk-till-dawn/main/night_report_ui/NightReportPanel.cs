using DuskTillDawn.Core.Night;
using Godot;

// The end-of-shift summary. Main decides when it shows; the panel only displays a report.
public partial class NightReportPanel : CanvasLayer
{
    [Export] public float OpenDuration = 0.45f;
    [Export] public float RevealDuration = 0.6f;
    [Export] public Color SurvivedColor = new(0.45f, 0.85f, 0.45f);
    [Export] public Color LostColor = new(0.9f, 0.35f, 0.3f);

    private Control _panel = null!;
    private BlockReveal _reveal = null!;
    private Label _title = null!;
    private Label _slaValue = null!;
    private Label _solvedValue = null!;
    private Label _failedValue = null!;
    private Label _pointsValue = null!;
    private Tween? _openTween;

    public override void _Ready()
    {
        _panel = GetNode<Control>("%Panel");
        _reveal = GetNode<BlockReveal>("%BlockReveal");
        _title = GetNode<Label>("%TitleLabel");
        _slaValue = GetNode<Label>("%SlaValue");
        _solvedValue = GetNode<Label>("%SolvedValue");
        _failedValue = GetNode<Label>("%FailedValue");
        _pointsValue = GetNode<Label>("%PointsValue");

        VisibilityChanged += () =>
        {
            if (Visible)
            {
                PlayOpen();
            }
        };
    }

    public void ShowReport(NightReport report)
    {
        _title.Text = report.Survived ? "Night report" : "Night lost";
        _title.Modulate = report.Survived ? SurvivedColor : LostColor;
        _slaValue.Text = $"{report.SlaLeft:0} %";
        _solvedValue.Text = report.Solved.ToString();
        _failedValue.Text = report.Failed.ToString();
        _pointsValue.Text = report.PointsEarned >= 0 ? $"+{report.PointsEarned}" : report.PointsEarned.ToString();
    }

    private void PlayOpen()
    {
        _openTween = UiMotion.PlayLiftOpen(_panel, _reveal, _openTween, OpenDuration, RevealDuration);
    }
}
