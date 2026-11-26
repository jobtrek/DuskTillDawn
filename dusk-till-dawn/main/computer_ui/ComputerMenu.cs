using System;
using Godot;

/// <summary>
/// Pixel-art NixOS desktop shown on the desk computer.
/// Every time it becomes visible it plays a "raise the map" animation:
/// the framed screen rises from the bottom, straightens, and the voxel blocks clear.
///
/// Hook your app menus to the signals below (MailPressed, MonitoringPressed, HelpPressed, AdminPressed).
/// </summary>
public partial class ComputerMenu : Control
{
    [Signal] public delegate void MailPressedEventHandler();
    [Signal] public delegate void MonitoringPressedEventHandler();
    [Signal] public delegate void HelpPressedEventHandler();
    [Signal] public delegate void AdminPressedEventHandler();

    [Export] public float OpenDuration = 0.45f;
    [Export] public float RevealDuration = 0.6f;

    private Control _map = null!;
    private ColorRect _backdrop = null!;
    private BlockReveal _reveal = null!;
    private Label _clock = null!;
    private PanelContainer _mailBadge = null!;
    private Label _mailBadgeLabel = null!;
    private Tween? _openTween;

    public override void _Ready()
    {
        _map = GetNode<Control>("Map");
        _backdrop = GetNode<ColorRect>("Backdrop");
        _reveal = GetNode<BlockReveal>("Map/Screen/BlockReveal");
        _clock = GetNode<Label>("Map/Screen/TopBar/Clock");

        WireIcon("Map/Screen/Icons/MailApp", () => EmitSignal(SignalName.MailPressed));
        WireIcon("Map/Screen/Icons/MonitoringApp", () => EmitSignal(SignalName.MonitoringPressed));
        WireIcon("Map/Screen/Icons/HelpApp", () => EmitSignal(SignalName.HelpPressed));
        WireIcon("Map/Screen/Icons/AdminApp", () => EmitSignal(SignalName.AdminPressed));
        BuildMailBadge(GetNode<TextureButton>("Map/Screen/Icons/MailApp/Button"));

        // Rotate/scale around the bottom-centre, like lifting a map into view.
        _map.Resized += () => _map.PivotOffset = new Vector2(_map.Size.X / 2f, _map.Size.Y);
        VisibilityChanged += () =>
        {
            if (IsVisibleInTree())
            {
                PlayOpen();
            }
        };
    }

    public override void _Process(double delta)
    {
        if (IsVisibleInTree())
        {
            _clock.Text = Time.GetTimeStringFromSystem()[..5];
        }
    }

    // The player sees unread mail from the desktop, before opening the Mail app.
    public void SetMailUnread(int count)
    {
        _mailBadge.Visible = count > 0;
        _mailBadgeLabel.Text = count > 99 ? "99+" : count.ToString();
    }

    private void BuildMailBadge(TextureButton icon)
    {
        _mailBadge = new PanelContainer { MouseFilter = MouseFilterEnum.Ignore, Visible = false };
        _mailBadge.AddThemeStyleboxOverride("panel", new StyleBoxFlat
        {
            BgColor = new Color(0.85f, 0.2f, 0.2f),
            BorderColor = new Color(0.06f, 0.06f, 0.08f),
            BorderWidthLeft = 2,
            BorderWidthTop = 2,
            BorderWidthRight = 2,
            BorderWidthBottom = 2,
            ContentMarginLeft = 4,
            ContentMarginRight = 4,
        });

        _mailBadgeLabel = new Label
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            MouseFilter = MouseFilterEnum.Ignore,
        };
        _mailBadgeLabel.AddThemeFontSizeOverride("font_size", 13);
        _mailBadgeLabel.AddThemeColorOverride("font_shadow_color", new Color(0, 0, 0, 0.85f));
        _mailBadgeLabel.AddThemeConstantOverride("shadow_offset_x", 2);
        _mailBadgeLabel.AddThemeConstantOverride("shadow_offset_y", 2);
        _mailBadge.AddChild(_mailBadgeLabel);

        icon.AddChild(_mailBadge);
        _mailBadge.SetAnchorsPreset(LayoutPreset.TopRight);
        _mailBadge.GrowHorizontal = GrowDirection.Begin;
    }

    private void WireIcon(string path, Action onPressed)
    {
        var app = GetNode<Control>(path);
        var button = app.GetNode<TextureButton>("Button");
        button.Pressed += onPressed;
        button.MouseEntered += () => SetHover(button, true);
        button.MouseExited += () => SetHover(button, false);
    }

    private void SetHover(TextureButton button, bool hovered)
    {
        button.PivotOffset = button.Size / 2f;
        var tween = button.CreateTween().SetParallel();
        tween.TweenProperty(button, "scale", Vector2.One * (hovered ? 1.12f : 1f), 0.08);
        tween.TweenProperty(button, "self_modulate", hovered ? new Color(1.25f, 1.25f, 1.25f) : Colors.White, 0.08);
    }

    private void PlayOpen()
    {
        _openTween?.Kill();

        float lift = GetViewportRect().Size.Y * 0.6f;
        _map.OffsetTop = lift;
        _map.OffsetBottom = lift;
        _map.Scale = Vector2.One * 0.6f;
        _map.RotationDegrees = -8f;
        _backdrop.Modulate = Colors.Transparent;
        _reveal.Progress = 0f;

        _openTween = CreateTween().SetParallel();
        _openTween.TweenProperty(_backdrop, "modulate", Colors.White, OpenDuration);
        _openTween.TweenProperty(_map, "offset_top", 0f, OpenDuration)
            .SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
        _openTween.TweenProperty(_map, "offset_bottom", 0f, OpenDuration)
            .SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
        _openTween.TweenProperty(_map, "scale", Vector2.One, OpenDuration)
            .SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
        _openTween.TweenProperty(_map, "rotation_degrees", 0f, OpenDuration)
            .SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
        _openTween.TweenProperty(_reveal, "Progress", 1f, RevealDuration)
            .SetDelay(OpenDuration * 0.5f);
    }
}
