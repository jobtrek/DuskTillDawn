# Build a new UI component

Part of the [GUI guide](README.md).

## Steps

1. **Decide the kind.** Full panel (menu, dialog, window, inventory, app screen) or small element (prompt, toast, HUD counter). Small elements skip step 6's reveal.
2. **Root.** A `Control`, full rect, `texture_filter = 1`.
3. **Neighbours.** List the existing UI that can be on screen with this one and plan its place, layer and input around them ([coexist.md](coexist.md)).
4. **Tree.** Build the standard panel structure from [structure.md](structure.md), sized with anchors.
5. **Textures.** Reuse `frame.png`. Draw anything new at tiny pixel sizes ([look.md](look.md)) into the component's `textures/` folder.
6. **Motion.** Open with `UiMotion.PlayLiftOpen` (full-screen) or `UiMotion.PlayWindowOpen` (App window) and a `BlockReveal`, and call `UiMotion.AddHover` on every button ([motion.md](motion.md)).
7. **Signals.** One `[Signal]` per action. Connect show/hide in the trigger script, not in the component.
8. **Build.** Run `dotnet build` in `dusk-till-dawn/`. Done when it builds with no errors.
9. **Review.** Run every check in [review.md](review.md) on the new component. Done when all pass.

## Reusable pieces

| Piece | Where | Use for |
|---|---|---|
| `BlockReveal.cs` | `main/computer_ui/` | The block-clearing reveal |
| `UiMotion.PlayLiftOpen` / `PlayWindowOpen` | `main/UiMotion.cs` | Open animation |
| `UiMotion.AddHover` | `main/UiMotion.cs` | Hover zoom on buttons |
| `frame.png` | `main/computer_ui/textures/` | Default panel frame (NinePatch) |

When a second component needs one of these, move it to `main/` (as `UiMotion.cs` was) and point both components at it, instead of copying it.

## C# conventions

- Get nodes in `_Ready()`; fields declared `= null!`.
- Signals: `[Signal] public delegate void XPressedEventHandler();`, emitted with `EmitSignal(SignalName.XPressed)`.
- Tunables (durations, sizes, colours) as `[Export]`.
- Tweens: `CreateTween().SetParallel()`. Animate offsets, scale, rotation and modulate, not positions set every frame.
- Braces on every `if`, `for`, `while`, even one-liners (project rule).
