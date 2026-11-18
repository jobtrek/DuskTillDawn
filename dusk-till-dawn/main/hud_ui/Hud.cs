using System.Collections.Generic;
using DuskTillDawn.Core;
using Godot;

/// <summary>
/// The always-on HUD: a framed status bar (clock, SLA, points) and the end-of-night banner.
/// Main calls ShowState every frame. The HUD only displays the state; it never changes the game.
/// </summary>
public partial class Hud : CanvasLayer
{
    private const double NightSurvivedBannerSeconds = 5;
    private const double NightStartHour = 20;
    private const double NightEndHour = 8;
    private const double SlaDangerPercent = 25;
    private const string LostMessage = "SLA at 0 %: the company is bankrupt";

    [Export] public float AppearDuration = 0.4f;
    [Export] public float PopDuration = 0.25f;
    [Export] public float BannerDuration = 0.35f;
    [Export] public float ToastSeconds = 3f;
    [Export] public float ToastFadeDuration = 0.4f;
    [Export] public Texture2D SunIcon = null!;
    [Export] public Texture2D MoonIcon = null!;

    private static readonly Color LossTint = new(1f, 0.45f, 0.4f);
    private static readonly Color GainTint = new(0.5f, 1f, 0.55f);
    private static readonly Color Flash = new(1.6f, 1.6f, 1.6f);

    private Control _statusBar = null!;
    private TextureRect _clockIcon = null!;
    private Label _clockValue = null!;
    private Label _slaValue = null!;
    private SlaBar _slaBar = null!;
    private Label _pointsValue = null!;
    private Control _banner = null!;
    private Label _bannerLabel = null!;
    private Control _mailToast = null!;
    private Label _mailToastLabel = null!;

    private readonly Dictionary<CanvasItem, Tween> _popTweens = new();
    private Tween? _bannerTween;
    private Tween? _toastTween;
    // Mails the current toast is announcing; a second arrival while it shows adds up instead of hiding the first.
    private int _toastMails;
    private bool? _isNight;
    private double _sla = double.NaN;
    private int? _points;
    private string? _message;
    private bool _slaDanger;
    private double _pulseTime;

    public override void _Ready()
    {
        _statusBar = GetNode<Control>("%StatusBar");
        _clockIcon = GetNode<TextureRect>("%ClockIcon");
        _clockValue = GetNode<Label>("%ClockValue");
        _slaValue = GetNode<Label>("%SlaValue");
        _slaBar = GetNode<SlaBar>("%SlaBar");
        _pointsValue = GetNode<Label>("%PointsValue");
        _banner = GetNode<Control>("%Banner");
        _bannerLabel = GetNode<Label>("%BannerLabel");
        _mailToast = GetNode<Control>("%MailToast");
        _mailToastLabel = GetNode<Label>("%MailToastLabel");

        _banner.Resized += () => _banner.PivotOffset = _banner.Size / 2f;
        _mailToast.Resized += () => _mailToast.PivotOffset = _mailToast.Size / 2f;
        PlayAppear();
    }

    public override void _Process(double delta)
    {
        if (!_slaDanger)
        {
            return;
        }

        // Low SLA: the number pulses red.
        _pulseTime += delta;
        float t = (Mathf.Sin((float)_pulseTime * 6f) + 1f) / 2f;
        _slaValue.SelfModulate = Colors.White.Lerp(_slaBar.BadColor, t);
    }

    public void ShowState(GameState state)
    {
        ShowClock(state);
        ShowSla(state.Sla);
        ShowPoints(state.Points);
        ShowBanner(EndMessage(state));
    }

    /// <summary>
    /// Tells the player mail came in, wherever they are in the building.
    /// Only the count, never sender or subject: the player has to go to the computer to judge it.
    /// </summary>
    public void ShowMailToast(int newMails)
    {
        bool showing = _mailToast.Visible;
        _toastMails = showing ? _toastMails + newMails : newMails;
        _mailToastLabel.Text = _toastMails == 1 ? "New mail" : $"{_toastMails} new mails";

        _toastTween?.Kill();
        _mailToast.Visible = true;
        _mailToast.PivotOffset = _mailToast.Size / 2f;
        // Already on screen: a small bump, so the new count gets noticed without a full re-entry.
        _mailToast.Scale = Vector2.One * (showing ? 1.15f : 0.6f);
        if (!showing)
        {
            _mailToast.Modulate = Colors.Transparent;
        }

        _toastTween = CreateTween().SetParallel();
        _toastTween.TweenProperty(_mailToast, "scale", Vector2.One, PopDuration)
            .SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
        _toastTween.TweenProperty(_mailToast, "modulate", Colors.White, PopDuration * 0.6f);
        _toastTween.Chain().TweenInterval(ToastSeconds);
        _toastTween.Chain().TweenProperty(_mailToast, "modulate", Colors.Transparent, ToastFadeDuration);
        _toastTween.Chain().TweenCallback(Callable.From(HideMailToast));
    }

    public void HideMailToast()
    {
        _toastTween?.Kill();
        _mailToast.Visible = false;
        _toastMails = 0;
    }

    private void ShowClock(GameState state)
    {
        _clockValue.Text = state.ClockText;

        double hour = state.HourOfDay;
        bool night = hour >= NightStartHour || hour < NightEndHour;
        if (night == _isNight)
        {
            return;
        }

        bool firstShow = _isNight == null;
        _isNight = night;
        _clockIcon.Texture = night ? MoonIcon : SunIcon;
        if (!firstShow)
        {
            Pop(_clockIcon, Colors.White);
        }
    }

    private void ShowSla(double sla)
    {
        if (sla == _sla)
        {
            return;
        }

        bool dropped = sla < _sla;
        _sla = sla;
        _slaValue.Text = $"{sla:0.0} %";
        _slaBar.Value = sla;

        _slaDanger = sla < SlaDangerPercent;
        if (!_slaDanger)
        {
            _slaValue.SelfModulate = Colors.White;
        }

        if (dropped)
        {
            Pop(_slaBar, Flash, 1f);
        }
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
            Pop(_pointsValue, points < previous ? LossTint : GainTint);
        }
    }

    private void ShowBanner(string? message)
    {
        if (message == _message)
        {
            return;
        }

        _message = message;
        _bannerTween?.Kill();
        _bannerTween = CreateTween().SetParallel();

        if (message == null)
        {
            _bannerTween.TweenProperty(_banner, "modulate", Colors.Transparent, BannerDuration * 0.6f);
            _bannerTween.Chain().TweenCallback(Callable.From(() => _banner.Visible = false));
            return;
        }

        _bannerLabel.Text = message;
        _bannerLabel.SelfModulate = message == LostMessage ? LossTint : Colors.White;
        _banner.Visible = true;
        _banner.PivotOffset = _banner.Size / 2f;
        _banner.Scale = Vector2.One * 0.6f;
        _banner.RotationDegrees = -4f;
        _banner.Modulate = Colors.Transparent;

        _bannerTween.TweenProperty(_banner, "modulate", Colors.White, BannerDuration * 0.6f);
        _bannerTween.TweenProperty(_banner, "scale", Vector2.One, BannerDuration)
            .SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
        _bannerTween.TweenProperty(_banner, "rotation_degrees", 0f, BannerDuration)
            .SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
    }

    // The status bar drops in from above the screen when the game starts.
    private void PlayAppear()
    {
        _statusBar.OffsetTop = -80f;
        _statusBar.OffsetBottom = -80f;
        _statusBar.Modulate = Colors.Transparent;

        var tween = CreateTween().SetParallel();
        tween.TweenProperty(_statusBar, "offset_top", 0f, AppearDuration)
            .SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
        tween.TweenProperty(_statusBar, "offset_bottom", 0f, AppearDuration)
            .SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
        tween.TweenProperty(_statusBar, "modulate", Colors.White, AppearDuration * 0.6f);
    }

    // Quick scale pop with a tint that fades back to white.
    private void Pop(Control item, Color tint, float scaleFrom = 1.3f)
    {
        if (_popTweens.TryGetValue(item, out Tween? previous))
        {
            previous.Kill();
        }

        item.PivotOffset = item.Size / 2f;
        item.Scale = Vector2.One * scaleFrom;
        item.Modulate = tint;

        Tween tween = item.CreateTween().SetParallel();
        tween.TweenProperty(item, "scale", Vector2.One, PopDuration)
            .SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
        tween.TweenProperty(item, "modulate", Colors.White, PopDuration);
        _popTweens[item] = tween;
    }

    private static string? EndMessage(GameState state)
    {
        if (state.Status == NightStatus.Lost)
        {
            return LostMessage;
        }

        if (state.DaySurvived)
        {
            return "Day 1 survived";
        }

        // Shown for a few seconds at 08:00, then the day goes on without the banner.
        return state.Status == NightStatus.Won && state.RealSecondsSinceNightEnded < NightSurvivedBannerSeconds
            ? "You survived the night"
            : null;
    }
}
