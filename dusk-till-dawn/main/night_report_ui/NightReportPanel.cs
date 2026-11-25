using DuskTillDawn.Core;
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

        _panel.Resized += () => _panel.PivotOffset = new Vector2(_panel.Size.X / 2f, _panel.Size.Y);
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
        _openTween?.Kill();

        float lift = GetViewport().GetVisibleRect().Size.Y * 0.6f;
        _panel.OffsetTop = lift;
        _panel.OffsetBottom = lift;
        _panel.Scale = Vector2.One * 0.6f;
        _panel.RotationDegrees = -8f;
        _reveal.Progress = 0f;

        _openTween = _panel.CreateTween().SetParallel();
        _openTween.TweenProperty(_panel, "offset_top", 0f, OpenDuration)
            .SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
        _openTween.TweenProperty(_panel, "offset_bottom", 0f, OpenDuration)
            .SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
        _openTween.TweenProperty(_panel, "scale", Vector2.One, OpenDuration)
            .SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
        _openTween.TweenProperty(_panel, "rotation_degrees", 0f, OpenDuration)
            .SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
        _openTween.TweenProperty(_reveal, "Progress", 1f, RevealDuration)
            .SetDelay(OpenDuration * 0.5f);
    }
}
