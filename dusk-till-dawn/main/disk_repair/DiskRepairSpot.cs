using DuskTillDawn.Core;
using Godot;

// The rack in the server room. E swaps a broken disk (the red disk shows), restarts the failed AC,
// or reboots the servers after a heat Auto-shutdown once the room has cooled down.
public partial class DiskRepairSpot : Node3D
{
    private const string RepairAction = "repair";

    [Export] public Node3D HealthyDisk { get; set; } = null!;
    [Export] public Node3D FailedDisk { get; set; } = null!;

    private Game _game = null!;
    private ServerRoom _serverRoom = null!;
    private InteractionPrompt _prompt = null!;
    private bool _playerInRange;

    public override void _Ready()
    {
        var main = (Main)GetTree().CurrentScene;
        _game = main.Game;
        _serverRoom = main.State.ServerRoom;
        _prompt = GetNode<InteractionPrompt>("InteractionPrompt");

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
        bool broken = HasBrokenDisk();
        ShowDisk(broken);

        string? prompt = null;
        if (broken)
        {
            prompt = "Press E to replace the disk";
        }
        else if (_game.IsRunning<OverheatingServer>())
        {
            prompt = "Press E to restart the cooling";
        }
        else if (_serverRoom.CanReboot)
        {
            prompt = "Press E to reboot the servers";
        }
        else if (_serverRoom.Status == ServerStatus.ShutDownByHeat)
        {
            prompt = $"Too hot to reboot: wait until {ServerRoom.ThrottleHeat:0} °C";
        }

        _prompt.Visible = _playerInRange && prompt != null;
        if (!_prompt.Visible)
        {
            return;
        }

        _prompt.SetText(prompt!);
        if (Input.IsActionJustPressed(RepairAction))
        {
            UseRack(broken);
        }
    }

    private void UseRack(bool broken)
    {
        if (broken)
        {
            ReplaceDisk();
        }
        else if (_game.IsRunning<OverheatingServer>())
        {
            _game.Repair(_game.Players[0], PlayerAction.RestartCooling);
        }
        else
        {
            _serverRoom.TryReboot();
        }
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
        }
    }

    private bool HasBrokenDisk()
    {
        for (var i = 0; i < _game.Challenges.Count; i++)
        {
            Challenge challenge = _game.Challenges[i];
            if (challenge is BrokenDisk && challenge.State == ChallengeState.Running)
            {
                return true;
            }
        }

        return false;
    }

    // Each challenge decides if a new disk fixes it, so only the broken disk reacts.
    private void ReplaceDisk()
    {
        PlayerProfile player = _game.Players[0];
        for (var i = 0; i < _game.Challenges.Count; i++)
        {
            _game.Challenges[i].Repair(player, PlayerAction.ReplaceDisk);
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
