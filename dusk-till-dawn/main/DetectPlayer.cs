using Godot;
using System;

public partial class DetectPlayer : MeshInstance3D
{
	// Called when the node enters the scene tree for the first time.

	private Control _control;
	public override void _Ready()
	{
		// the equivilent of getElementById in JS.
		var _area3d = GetNode<Area3D>("Area3D");
		_control = GetNode<Control>("Control");
		_control.Hide();

		// so basically _area3d.BodyEntered is a signal that has to be added to a function, and the function takes an input as a 3d node.. I SEEE 
		_area3d.BodyEntered += OnBodyEntered;
		_area3d.BodyExited += OnBodyLeave;

	}

	private void OnBodyEntered(Node3D body)
	{
		_control.Show();
		// so instead of basically displaying print to the console we need to display a simple gui in front of the user.

		GD.Print($"{body.Name} entered the zone");
	}

	private void OnBodyLeave(Node3D body)
	{
		_control.Hide();
		GD.Print($"{body.Name} has left the zone");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}