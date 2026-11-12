using Godot;

public partial class DetectPlayerNearComputer : MeshInstance3D
{
	private Control _control;
	private Control _control2;
	private bool _playerInRange;

	/* so if the player walks near the zone then it should display the press e to open the computer, then the user needs to clicks 'E' 
	it closes the original menu, opens the new one.

	I'll code the simple thing first, when the player leaves -> everything closes.
	 */

	public override void _Ready()
	{
		var area3D = GetNode<Area3D>("Area3D");
		_control = GetNode<Control>("UserHelper");
		_control2 = GetNode<Control>("Menu");

		_control.Hide();
		_control2.Hide();


		// these add the signals if the player has entered or left the zone
		area3D.BodyEntered += OnBodyEntered;
		area3D.BodyExited += OnBodyExited;

		// this verifies that there isn't an action and creates a new Action and assigns an event to said action.
		if (!InputMap.HasAction("open_menu"))
			InputMap.AddAction("open_menu");
		InputMap.ActionAddEvent("open_menu", new InputEventKey { PhysicalKeycode = Key.E });
	}

	private void OnBodyEntered(Node3D body)
	{
		_playerInRange = true;
		_control.Show();
	}

	private void OnBodyExited(Node3D body)
	{
		_playerInRange = false;
		_control.Hide();
		_control2.Hide();
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("open_menu"))
		{
			_control2.Show();
			_control.Hide();
		}

	}
}