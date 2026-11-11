using DuskTillDawn.Core;
using Godot;

public partial class Main : Node3D
{
    private readonly GameState _state = new();
    private Hud _hud = null!;

    public override void _Ready()
    {
        _hud = GetNode<Hud>("Hud");
        _hud.ShowState(_state);
    }

    public override void _Process(double delta)
    {
        _state.Tick(delta);
        ShowStateAndStopIfNightEnded();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (!OS.IsDebugBuild() || @event is not InputEventKey { Pressed: true } key)
        {
            return;
        }

        if (key.Keycode == Key.F9)
        {
            _state.DrainSla(_state.Sla);
        }
        else if (key.Keycode == Key.F10)
        {
            _state.Tick(double.MaxValue);
        }

        ShowStateAndStopIfNightEnded();
    }

    private void ShowStateAndStopIfNightEnded()
    {
        _hud.ShowState(_state);
        GetTree().Paused = _state.Status != NightStatus.Running;
    }
}
