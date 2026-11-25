using Godot;

// Admin console window: where the player does the fix once they know the cause
// (docs/specs/admin_console_spec.md): Firewall (traffic filtering) and Power (faulty circuit, Eco Mode,
// Graceful shutdown).
public partial class AdminApp : Control
{
    // action is a PlayerAction name, so the game can match it to the challenge it repairs.
    [Signal] public delegate void ActionPressedEventHandler(string system, string action);
    [Signal] public delegate void CloseRequestedEventHandler();

    [Export] public float OpenDuration = 0.25f;
    [Export] public float RevealDuration = 0.5f;

    private BlockReveal _reveal = null!;
    private Tween? _openTween;
    private Label _powerStatus = null!;
    private Godot.Button _ecoMode = null!;
    private Godot.Button _gracefulShutdown = null!;

    public override void _Ready()
    {
        _reveal = GetNode<BlockReveal>("BlockReveal");

        var close = GetNode<Godot.Button>("TitleBar/Close");
        close.Pressed += () => EmitSignal(SignalName.CloseRequested);
        AddHover(close);

        AddAction("Firewall", "EnableTrafficFilter");
        AddAction("Power", "CutFaultyCircuit");
        _ecoMode = AddAction("Power", "EcoMode");
        _gracefulShutdown = AddAction("Power", "GracefulShutdown");
        _powerStatus = GetNode<Label>("Body/Panel/Sections/Power/Status");

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

    public void ShowPower(string status, bool ecoModeOn, bool canTurnOnEcoMode, bool canShutDown)
    {
        _powerStatus.Text = status;
        _ecoMode.Text = ecoModeOn ? "Turn off Eco Mode" : "Eco Mode";
        SetEnabled(_ecoMode, ecoModeOn || canTurnOnEcoMode);
        SetEnabled(_gracefulShutdown, canShutDown);
    }

    private static void SetEnabled(Godot.Button button, bool enabled)
    {
        button.Disabled = !enabled;
        button.Modulate = enabled ? Colors.White : new Color(1f, 1f, 1f, 0.4f);
    }

    private Godot.Button AddAction(string system, string action)
    {
        var button = GetNode<Godot.Button>($"Body/Panel/Sections/{system}/{action}");
        button.Pressed += () => EmitSignal(SignalName.ActionPressed, system, action);
        AddHover(button);
        return button;
    }

    private static void AddHover(Godot.Button button)
    {
        button.MouseEntered += () => SetHover(button, true);
        button.MouseExited += () => SetHover(button, false);
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
