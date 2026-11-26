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
        UiMotion.AddHover(button);
    }

    private void PlayOpen()
    {
        _openTween = UiMotion.PlayLiftOpen(_map, _reveal, _openTween, OpenDuration, RevealDuration, _backdrop);
    }
}
