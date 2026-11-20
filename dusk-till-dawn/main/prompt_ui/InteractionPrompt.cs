using Godot;

// A "Press E to ..." hint at the bottom of the screen. The zone that owns it shows and hides it;
// the prompt only fades in so it never pops abruptly over the world.
public partial class InteractionPrompt : CanvasLayer
{
    [Export] public string Text = "Press E";
    [Export] public float FadeDuration = 0.15f;

    private Control _root = null!;
    private Label _label = null!;
    private Tween? _fade;

    public override void _Ready()
    {
        _root = GetNode<Control>("Root");
        _label = GetNode<Label>("%PromptLabel");
        _label.Text = Text;
        VisibilityChanged += OnVisibilityChanged;
    }

    public void SetText(string text)
    {
        Text = text;
        _label.Text = text;
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
