using Godot;

// Admin console window: where the player does the fix once they know the cause
// (docs/specs/admin_console_spec.md). v1 only has Firewall → Enable traffic filtering.
public partial class AdminApp : Control
{
    // action is a PlayerAction name, so the game can match it to the challenge it repairs.
    [Signal] public delegate void ActionPressedEventHandler(string system, string action);
    [Signal] public delegate void CloseRequestedEventHandler();

    [Export] public float OpenDuration = 0.25f;
    [Export] public float RevealDuration = 0.5f;

    private BlockReveal _reveal = null!;
    private Tween? _openTween;

    public override void _Ready()
    {
        _reveal = GetNode<BlockReveal>("BlockReveal");

        var close = GetNode<Godot.Button>("TitleBar/Close");
        close.Pressed += () => EmitSignal(SignalName.CloseRequested);
        AddHover(close);

        var filter = GetNode<Godot.Button>("Body/Panel/Firewall/EnableTrafficFilter");
        filter.Pressed += () => EmitSignal(SignalName.ActionPressed, "Firewall", "EnableTrafficFilter");
        AddHover(filter);

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
