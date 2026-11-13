using System.Collections.Generic;
using Godot;

public enum Metric
{
    Power,
    Heat,
    Traffic,
    Disk,
    Sla,
}

/// <summary>
/// Monitoring window shown inside the desk computer screen.
/// One row per <see cref="Metric"/>: name, a danger bar, the real value.
///
/// The UI only shows values and emits signals. <see cref="MonitoringController"/> reads GameState,
/// computes the danger t (0 = good, 1 = bad) and calls <see cref="SetMetric"/>.
/// </summary>
public partial class MonitorApp : Control
{
    [Signal] public delegate void CloseRequestedEventHandler();

    [Export] public float OpenDuration = 0.25f;
    [Export] public float RevealDuration = 0.5f;

    private static readonly Color Track = new(1f, 1f, 1f, 0.08f);
    private static readonly Color Green = new(0.45f, 0.85f, 0.45f);
    private static readonly Color Orange = new(0.95f, 0.65f, 0.25f);
    private static readonly Color Red = new(0.95f, 0.4f, 0.35f);

    private readonly Dictionary<Metric, Row> _rows = new();
    private VBoxContainer _list = null!;
    private BlockReveal _reveal = null!;
    private Tween? _openTween;

    private sealed record Row(ColorRect Fill, Label Value, string Unit);

    public override void _Ready()
    {
        _list = GetNode<VBoxContainer>("Body/Rows");
        _reveal = GetNode<BlockReveal>("BlockReveal");

        AddRow(Metric.Power, "Power", "%");
        AddRow(Metric.Heat, "Heat", "°C");
        AddRow(Metric.Traffic, "Traffic", "%");
        AddRow(Metric.Disk, "Disk", "%");
        AddRow(Metric.Sla, "SLA", "%");

        var close = GetNode<Godot.Button>("TitleBar/Close");
        close.Pressed += () => EmitSignal(SignalName.CloseRequested);
        close.MouseEntered += () => SetHover(close, true);
        close.MouseExited += () => SetHover(close, false);

        Resized += () => PivotOffset = Size / 2f;
        VisibilityChanged += () =>
        {
            if (IsVisibleInTree())
            {
                PlayOpen();
            }
        };
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (IsVisibleInTree() && @event.IsActionPressed("ui_cancel"))
        {
            GetViewport().SetInputAsHandled();
            EmitSignal(SignalName.CloseRequested);
        }
    }

    /// <summary>Shows the real value as text; danger (0..1) drives the bar fill and colour.</summary>
    public void SetMetric(Metric metric, double value, float danger)
    {
        var row = _rows[metric];
        danger = Mathf.Clamp(danger, 0f, 1f);

        var color = DangerColor(danger);
        row.Value.Text = $"{value:0} {row.Unit}";
        row.Value.Modulate = color;
        row.Fill.AnchorRight = danger;
        row.Fill.Color = color;
    }

    private static Color DangerColor(float t)
    {
        if (t < 0.5f)
        {
            return Green.Lerp(Orange, t * 2f);
        }
        return Orange.Lerp(Red, (t - 0.5f) * 2f);
    }

    private void AddRow(Metric metric, string name, string unit)
    {
        var line = new HBoxContainer { MouseFilter = MouseFilterEnum.Ignore };
        line.AddThemeConstantOverride("separation", 10);
        line.SizeFlagsVertical = SizeFlags.ExpandFill;

        var label = MakeLabel(name, 14);
        label.CustomMinimumSize = new Vector2(70, 0);
        line.AddChild(label);

        var bar = new ColorRect
        {
            Color = Track,
            CustomMinimumSize = new Vector2(0, 14),
            SizeFlagsHorizontal = SizeFlags.ExpandFill,
            SizeFlagsVertical = SizeFlags.ShrinkCenter,
            MouseFilter = MouseFilterEnum.Ignore,
        };
        var fill = new ColorRect { Color = Green, MouseFilter = MouseFilterEnum.Ignore };
        bar.AddChild(fill);
        fill.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
        fill.AnchorRight = 0f;
        line.AddChild(bar);

        var value = MakeLabel("", 14);
        value.CustomMinimumSize = new Vector2(64, 0);
        value.HorizontalAlignment = HorizontalAlignment.Right;
        line.AddChild(value);

        _list.AddChild(line);
        _rows[metric] = new Row(fill, value, unit);
    }

    private static Label MakeLabel(string text, int size)
    {
        var label = new Label
        {
            Text = text,
            VerticalAlignment = VerticalAlignment.Center,
            MouseFilter = MouseFilterEnum.Ignore,
        };
        label.AddThemeFontSizeOverride("font_size", size);
        return label;
    }

    private static void SetHover(Godot.Button button, bool hovered)
    {
        button.PivotOffset = button.Size / 2f;
        var tween = button.CreateTween().SetParallel();
        tween.TweenProperty(button, "scale", Vector2.One * (hovered ? 1.12f : 1f), 0.08);
        tween.TweenProperty(button, "self_modulate", hovered ? new Color(1.25f, 1.25f, 1.25f) : Colors.White, 0.08);
    }

    private void PlayOpen()
    {
        _openTween?.Kill();

        Scale = Vector2.One * 0.85f;
        Modulate = Colors.Transparent;
        _reveal.Progress = 0f;

        _openTween = CreateTween().SetParallel();
        _openTween.TweenProperty(this, "modulate", Colors.White, OpenDuration * 0.6f);
        _openTween.TweenProperty(this, "scale", Vector2.One, OpenDuration)
            .SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
        _openTween.TweenProperty(_reveal, "Progress", 1f, RevealDuration)
            .SetDelay(OpenDuration * 0.4f);
    }
}
