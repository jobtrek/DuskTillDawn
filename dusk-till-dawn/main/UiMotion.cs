using System.Collections.Generic;
using Godot;

// The shared motion of the voxel UI style (agent_context/gui/motion.md): tweak a look here once,
// every window, the HUD and the Dashboard follow.
public static class UiMotion
{
    public static readonly Color LossTint = new(1f, 0.45f, 0.4f);
    public static readonly Color GainTint = new(0.5f, 1f, 0.55f);

    private const float HoverScale = 1.12f;
    private const double HoverDuration = 0.08;
    private static readonly Color HoverBrighten = new(1.25f, 1.25f, 1.25f);

    public static void AddHover(Control button)
    {
        button.MouseEntered += () => SetHover(button, true);
        button.MouseExited += () => SetHover(button, false);
    }

    public static void SetHover(Control button, bool hovered)
    {
        button.PivotOffset = button.Size / 2f;
        var tween = button.CreateTween().SetParallel();
        tween.TweenProperty(button, "scale", Vector2.One * (hovered ? HoverScale : 1f), HoverDuration);
        tween.TweenProperty(button, "self_modulate", hovered ? HoverBrighten : Colors.White, HoverDuration);
    }

    // App window inside the desk computer: fades and bounces up to full size, then the blocks clear.
    public static Tween PlayWindowOpen(Control window, BlockReveal reveal, Tween? previous,
        float openDuration, float revealDuration)
    {
        previous?.Kill();

        window.Scale = Vector2.One * 0.85f;
        window.Modulate = Colors.Transparent;
        reveal.Progress = 0f;

        var tween = window.CreateTween().SetParallel();
        tween.TweenProperty(window, "modulate", Colors.White, openDuration * 0.6f);
        tween.TweenProperty(window, "scale", Vector2.One, openDuration)
            .SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
        tween.TweenProperty(reveal, "Progress", 1f, revealDuration)
            .SetDelay(openDuration * 0.4f);
        return tween;
    }

    // Full-screen panel (desk computer desktop, night report): lifted into view like a map, then the blocks clear.
    public static Tween PlayLiftOpen(Control panel, BlockReveal reveal, Tween? previous,
        float openDuration, float revealDuration, CanvasItem? backdrop = null)
    {
        previous?.Kill();

        float lift = panel.GetViewportRect().Size.Y * 0.6f;
        panel.PivotOffset = new Vector2(panel.Size.X / 2f, panel.Size.Y);
        panel.OffsetTop = lift;
        panel.OffsetBottom = lift;
        panel.Scale = Vector2.One * 0.6f;
        panel.RotationDegrees = -8f;
        reveal.Progress = 0f;

        var tween = panel.CreateTween().SetParallel();
        if (backdrop != null)
        {
            backdrop.Modulate = Colors.Transparent;
            tween.TweenProperty(backdrop, "modulate", Colors.White, openDuration);
        }
        TweenToZero(tween, panel, "offset_top", openDuration);
        TweenToZero(tween, panel, "offset_bottom", openDuration);
        tween.TweenProperty(panel, "scale", Vector2.One, openDuration)
            .SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
        tween.TweenProperty(panel, "rotation_degrees", 0f, openDuration)
            .SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
        tween.TweenProperty(reveal, "Progress", 1f, revealDuration)
            .SetDelay(openDuration * 0.5f);
        return tween;
    }

    // Esc closes the open window; true means the caller should ask to close.
    public static bool IsCloseRequest(Control window, InputEvent @event)
    {
        if (!window.IsVisibleInTree() || !@event.IsActionPressed("ui_cancel"))
        {
            return false;
        }
        window.GetViewport().SetInputAsHandled();
        return true;
    }

    // HUD counter change: quick scale pop with a tint that fades back to white.
    public static void Pop(Control item, Color tint, float duration, Dictionary<Control, Tween> running,
        float scaleFrom = 1.3f)
    {
        if (running.TryGetValue(item, out Tween? previous))
        {
            previous.Kill();
        }

        item.PivotOffset = item.Size / 2f;
        item.Scale = Vector2.One * scaleFrom;
        item.Modulate = tint;

        Tween tween = item.CreateTween().SetParallel();
        tween.TweenProperty(item, "scale", Vector2.One, duration)
            .SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
        tween.TweenProperty(item, "modulate", Colors.White, duration);
        running[item] = tween;
    }

    // HUD panel entering the screen at game start, from `shift` pixels away from its resting place.
    public static void SlideIn(Control panel, Vector2 shift, float duration)
    {
        panel.Modulate = Colors.Transparent;
        var tween = panel.CreateTween().SetParallel();

        if (shift.X != 0f)
        {
            panel.OffsetLeft = shift.X;
            panel.OffsetRight = shift.X;
            TweenToZero(tween, panel, "offset_left", duration);
            TweenToZero(tween, panel, "offset_right", duration);
        }
        if (shift.Y != 0f)
        {
            panel.OffsetTop = shift.Y;
            panel.OffsetBottom = shift.Y;
            TweenToZero(tween, panel, "offset_top", duration);
            TweenToZero(tween, panel, "offset_bottom", duration);
        }
        tween.TweenProperty(panel, "modulate", Colors.White, duration * 0.6f);
    }

    private static void TweenToZero(Tween tween, Control panel, string offset, float duration)
    {
        tween.TweenProperty(panel, offset, 0f, duration)
            .SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
    }
}
