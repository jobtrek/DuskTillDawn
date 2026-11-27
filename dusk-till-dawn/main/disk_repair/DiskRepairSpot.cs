using DuskTillDawn.Core;
using Godot;

// The rack in the server room. E opens the RackChoice: the player picks the fix (swap the disk, restart the AC,
// reboot the servers after a heat Auto-shutdown) and a wrong repair makes them Busy.
public partial class DiskRepairSpot : Node3D
{
    private const string RepairAction = "repair";
    private const string RebootServers = "RebootServers";

    [Export] public Node3D HealthyDisk { get; set; } = null!;
    [Export] public Node3D FailedDisk { get; set; } = null!;

    private Game _game = null!;
    private ServerRoom _serverRoom = null!;
    private InteractionPrompt _prompt = null!;
    private RackChoice _choice = null!;
    private bool _playerInRange;

    public override void _Ready()
    {
        var main = (Main)GetTree().CurrentScene;
        _game = main.Game;
        _serverRoom = main.State.ServerRoom;
        _prompt = GetNode<InteractionPrompt>("InteractionPrompt");
        _choice = GetNode<RackChoice>("RackChoice");
        _choice.ChoicePressed += OnChoicePressed;
        _choice.CloseRequested += () => _choice.Hide();

        var zone = GetNode<Area3D>("Zone");
        zone.BodyEntered += OnBodyEntered;
        zone.BodyExited += OnBodyExited;

        if (!InputMap.HasAction(RepairAction))
        {
            InputMap.AddAction(RepairAction);
            InputMap.ActionAddEvent(RepairAction, new InputEventKey { PhysicalKeycode = Key.E });
        }

        ShowDisk(broken: false);
    }

    public override void _Process(double delta)
    {
        ShowDisk(_game.IsRunning<BrokenDisk>());

        // The choice covers the prompt's spot, so only one of them shows at a time.
        _prompt.Visible = _playerInRange && !_choice.Visible;
        if (!_prompt.Visible)
        {
            return;
        }

        PlayerProfile player = _game.Players[0];
        if (player.IsBusy)
        {
            _prompt.SetText(BusyText.For(player.BusySecondsLeft));
            return;
        }

        _prompt.SetText(_serverRoom.Status == ServerStatus.ShutDownByHeat && !_serverRoom.CanReboot
            ? $"Too hot to reboot: wait until {ServerRoom.ThrottleHeat:0} °C"
            : "Press E to use the rack");
        if (Input.IsActionJustPressed(RepairAction))
        {
            _choice.Show();
        }
    }

    private void OnChoicePressed(string action)
    {
        _choice.Hide();
        if (action == RebootServers)
        {
            _serverRoom.TryReboot();
            return;
        }

        if (!System.Enum.TryParse(action, out PlayerAction playerAction) || !System.Enum.IsDefined(playerAction))
        {
            GD.PushError($"Rack choice '{action}' is not a PlayerAction.");
            return;
        }

        _game.Repair(_game.Players[0], playerAction);
    }

    // The zone also overlaps the rack and the floor; only the player counts.
    private void OnBodyEntered(Node3D body)
    {
        if (body is CharacterBody3D)
        {
            _playerInRange = true;
        }
    }

    private void OnBodyExited(Node3D body)
    {
        if (body is CharacterBody3D)
        {
            _playerInRange = false;
            _choice.Hide();
        }
    }

    private void ShowDisk(bool broken)
    {
        if (HealthyDisk.Visible == !broken && FailedDisk.Visible == broken)
        {
            return;
        }

        HealthyDisk.Visible = !broken;
        FailedDisk.Visible = broken;
    }
}
