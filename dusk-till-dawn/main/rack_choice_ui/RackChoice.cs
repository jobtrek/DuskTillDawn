using Godot;

// The choice at the server room rack (docs/player-flow/challenges.md, Rack choice): the player picks the fix,
// the rack does not pick it for them. ReplaceDisk and RestartCooling are PlayerAction names; RebootServers is not
// a repair.
public partial class RackChoice : CanvasLayer
{
    [Signal] public delegate void ChoicePressedEventHandler(string action);
    [Signal] public delegate void CloseRequestedEventHandler();

    [Export] public float FadeDuration = 0.15f;

    private Control _root = null!;
    private Tween? _fade;

    public override void _Ready()
    {
        _root = GetNode<Control>("Root");
        AddChoice("ReplaceDisk");
        AddChoice("RestartCooling");
        AddChoice("RebootServers");
        VisibilityChanged += OnVisibilityChanged;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (Visible && UiMotion.TryConsumeClose(_root, @event))
        {
            EmitSignal(SignalName.CloseRequested);
        }
    }

    private void AddChoice(string action)
    {
        var button = GetNode<Godot.Button>($"Root/Panel/Content/Actions/{action}");
        button.Pressed += () => EmitSignal(SignalName.ChoicePressed, action);
        UiMotion.AddHover(button);
    }

    private void OnVisibilityChanged()
    {
        if (!Visible)
        {
            return;
        }

        _fade?.Kill();
        _root.Modulate = new Color(1, 1, 1, 0);
        _fade = CreateTween();
        _fade.TweenProperty(_root, "modulate:a", 1f, FadeDuration);
    }
}
