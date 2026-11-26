using System.Collections.Generic;
using Godot;
using DuskTillDawn.Core;

public enum Metric
{
    Power,
    Heat,
    Traffic,
    Disk,
    Sla,
}

// dusk-till-dawn/main/monitor_ui/MonitorApp.cs
// The window only shows values and emits signals: MonitoringController reads GameState and decides.
public partial class MonitorApp : Control
{
    [Signal] public delegate void CloseRequestedEventHandler();

    [Export] public float OpenDuration = 0.25f;
    [Export] public float RevealDuration = 0.5f;

    private static readonly Color Track = new(1f, 1f, 1f, 0.08f);
    private static readonly Color Green = new(0.45f, 0.85f, 0.45f);
    private static readonly Color Orange = new(0.95f, 0.65f, 0.25f);
    private static readonly Color Red = new(0.95f, 0.4f, 0.35f);
    private static readonly Color Muted = new(1f, 1f, 1f, 0.55f);
    private static readonly Color TabNormal = new(1f, 1f, 1f, 0.06f);
    private static readonly Color TabSelected = new(0.32f, 0.47f, 0.75f, 0.35f);

    private readonly Dictionary<Metric, Row> _rows = new();
    private readonly Dictionary<string, ServerRow> _serverRows = new();
    private readonly List<(Godot.Button Tab, Control Page)> _tabs = new();
    private VBoxContainer _list = null!;
    private VBoxContainer _servers = null!;
    private BlockReveal _reveal = null!;
    private Tween? _openTween;

    private sealed record Row(ColorRect Fill, Label Value, string Unit);
    private sealed record ServerRow(Label Status, Label Clients, Label Load);

    public override void _Ready()
    {
        _list = GetNode<VBoxContainer>("Body/Pages/Rows");
        _servers = GetNode<VBoxContainer>("Body/Pages/Servers");
        _reveal = GetNode<BlockReveal>("BlockReveal");

        AddRow(Metric.Power, "Power", "%");
        AddRow(Metric.Heat, "Heat", "°C");
        AddRow(Metric.Traffic, "Traffic", "%");
        AddRow(Metric.Disk, "Disk", "%");
        AddRow(Metric.Sla, "SLA", "%");

        _servers.AddChild(MakeServerLine("SERVER", "STATUS", "CLIENTS", "LOAD", Muted).Line);

        var tabs = GetNode<HBoxContainer>("Body/Pages/Tabs");
        AddTab(tabs, "Metrics", _list);
        AddTab(tabs, "Servers", _servers);
        SelectTab(_list);

        var close = GetNode<Godot.Button>("TitleBar/Close");
        close.Pressed += () => EmitSignal(SignalName.CloseRequested);
        UiMotion.AddHover(close);

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
        if (UiMotion.TryConsumeClose(this, @event))
        {
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

    public void SetServers(IReadOnlyList<Server> servers)
    {
        foreach (var server in servers)
        {
            if (!_serverRows.TryGetValue(server.Name, out var row))
            {
                var made = MakeServerLine(server.Name, "", "", "", Colors.White);
                _servers.AddChild(made.Line);
                row = made.Row;
                _serverRows[server.Name] = row;
            }

            var color = server.Status switch
            {
                ServerHealth.Down => Red,
                ServerHealth.Warn => Orange,
                _ => Green,
            };
            row.Status.Text = $"● {server.Status.ToString().ToUpperInvariant()}";
            row.Status.Modulate = color;
            row.Clients.Text = string.Join(", ", server.Clients);
            row.Load.Text = server.Status == ServerHealth.Down ? "—" : $"{server.Load * 100:0} %";
        }
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

    private static (HBoxContainer Line, ServerRow Row) MakeServerLine(
        string name, string status, string clients, string load, Color color)
    {
        var line = new HBoxContainer { MouseFilter = MouseFilterEnum.Ignore };
        line.AddThemeConstantOverride("separation", 10);

        var nameLabel = MakeLabel(name, 14);
        nameLabel.CustomMinimumSize = new Vector2(70, 0);
        var statusLabel = MakeLabel(status, 14);
        statusLabel.CustomMinimumSize = new Vector2(70, 0);
        var clientsLabel = MakeLabel(clients, 14);
        clientsLabel.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        clientsLabel.TextOverrunBehavior = TextServer.OverrunBehavior.TrimEllipsis;
        clientsLabel.ClipText = true;
        var loadLabel = MakeLabel(load, 14);
        loadLabel.CustomMinimumSize = new Vector2(50, 0);
        loadLabel.HorizontalAlignment = HorizontalAlignment.Right;

        foreach (var label in new[] { nameLabel, statusLabel, clientsLabel, loadLabel })
        {
            label.Modulate = color;
            line.AddChild(label);
        }
        return (line, new ServerRow(statusLabel, clientsLabel, loadLabel));
    }

    private void AddTab(HBoxContainer tabs, string text, Control page)
    {
        var tab = new Godot.Button
        {
            Text = text,
            CustomMinimumSize = new Vector2(90, 24),
            FocusMode = FocusModeEnum.None,
        };
        tab.AddThemeFontSizeOverride("font_size", 13);
        tab.AddThemeStyleboxOverride("focus", new StyleBoxEmpty());
        tab.Pressed += () => SelectTab(page);
        UiMotion.AddHover(tab);
        tabs.AddChild(tab);
        _tabs.Add((tab, page));
    }

    private void SelectTab(Control page)
    {
        foreach (var (tab, tabPage) in _tabs)
        {
            bool selected = tabPage == page;
            tabPage.Visible = selected;
            var box = new StyleBoxFlat { BgColor = selected ? TabSelected : TabNormal };
            foreach (var state in new[] { "normal", "hover", "pressed" })
            {
                tab.AddThemeStyleboxOverride(state, box);
            }
        }
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

    private void PlayOpen()
    {
        _openTween = UiMotion.PlayWindowOpen(this, _reveal, _openTween, OpenDuration, RevealDuration);
    }
}
