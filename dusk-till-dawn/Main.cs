using DuskTillDawn.Core;
using Godot;

public partial class Main : Node3D
{
    private readonly GameState _state = new();
    private Hud _hud = null!;
    private Player _player = null!;
    private Area3D _computerArea = null!;

    public override void _Ready()
    {
        _hud = GetNode<Hud>("Hud");
        _player = GetNode<Player>("Player");
        _computerArea = GetNode<Area3D>("Room/ComputerArea");
    }

    public override void _Process(double delta)
    {
        _state.SetPlayerNextToComputer(_computerArea.OverlapsBody(_player));
        _state.Tick(delta);

        _hud.ShowState(_state);
        _player.SetPhysicsProcess(_state.CanPlayerMove);
        GetTree().Paused = _state.Status != NightStatus.Running;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("interact"))
        {
            _state.ToggleComputer();
        }
        else if (@event.IsActionPressed("ui_cancel"))
        {
            _state.CloseComputer();
        }
        else if (OS.IsDebugBuild() && @event is InputEventKey { Pressed: true, Keycode: Key.F9 })
        {
            _state.DrainSla(_state.Sla);
        }
        else if (OS.IsDebugBuild() && @event is InputEventKey { Pressed: true, Keycode: Key.F10 })
        {
            _state.Tick(double.MaxValue);
        }
    }
}
