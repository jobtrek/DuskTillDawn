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
    private Godot.Button _trafficFilter = null!;
    private Godot.Button _cutFaultyCircuit = null!;
    private Label _busy = null!;

    public override void _Ready()
    {
        _reveal = GetNode<BlockReveal>("BlockReveal");

        var close = GetNode<Godot.Button>("TitleBar/Close");
        close.Pressed += () => EmitSignal(SignalName.CloseRequested);
        UiMotion.AddHover(close);

        _trafficFilter = AddAction("Firewall", "EnableTrafficFilter");
        _cutFaultyCircuit = AddAction("Power", "CutFaultyCircuit");
        _ecoMode = AddAction("Power", "EcoMode");
        _gracefulShutdown = AddAction("Power", "GracefulShutdown");
        _powerStatus = GetNode<Label>("Body/Panel/Sections/Power/Status");
        _busy = GetNode<Label>("Body/Panel/Sections/Busy");
        _busy.SelfModulate = UiMotion.LossTint;

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

    public void ShowPower(string status, bool ecoModeOn, bool canTurnOnEcoMode, bool canShutDown)
    {
        _powerStatus.Text = status;
        _ecoMode.Text = ecoModeOn ? "Turn off Eco Mode" : "Eco Mode";
        SetEnabled(_ecoMode, ecoModeOn || canTurnOnEcoMode);
        SetEnabled(_gracefulShutdown, canShutDown);
    }

    // A wrong guess costs the sysadmin time: repairs stay greyed until it runs out.
    // Eco Mode and Graceful shutdown are not repairs, so they stay as ShowPower set them.
    public void ShowBusy(double secondsLeft)
    {
        bool busy = secondsLeft > 0;
        _busy.Visible = busy;
        if (busy)
        {
            _busy.Text = BusyText.For(secondsLeft);
        }

        SetEnabled(_trafficFilter, !busy);
        SetEnabled(_cutFaultyCircuit, !busy);
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
        UiMotion.AddHover(button);
        return button;
    }

    private void PlayOpen()
    {
        _openTween = UiMotion.PlayWindowOpen(this, _reveal, _openTween, OpenDuration, RevealDuration);
    }
}
