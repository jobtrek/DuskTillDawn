using DuskTillDawn.Core;
using Godot;

public partial class Main : Node3D
{
	private readonly GameState _state = new();
	private Hud _hud = null!;

	public override void _Ready()
	{
		_hud = GetNode<Hud>("Hud");
		ShowStateAndPauseIfGameEnded();
		AddMissingStairCollision(this);
	}

	// The imported buildings ship without collision on some "Stairs_*" meshes.
	private static void AddMissingStairCollision(Node node)
	{
		foreach (Node child in node.GetChildren())
		{
			if (child is MeshInstance3D { Mesh: not null } mesh
				&& mesh.Name.ToString().StartsWith("Stairs_")
				&& mesh.GetNodeOrNull<StaticBody3D>("StaticBody3D") == null)
			{
				mesh.CreateConvexCollision(); // hull of a staircase = a smooth ramp
			}

			AddMissingStairCollision(child);
		}
	}

	public override void _Process(double delta)
	{
		_state.Tick(delta);
		ShowStateAndPauseIfGameEnded();
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

		ShowStateAndPauseIfGameEnded();
	}

	private void ShowStateAndPauseIfGameEnded()
	{
		_hud.ShowState(_state);
		GetTree().Paused = _state.Status == NightStatus.Lost || _state.DaySurvived;
	}
}
