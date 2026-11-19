using DuskTillDawn.Core;
using Godot;

// The rack in the server room where a broken disk gets swapped. While a BrokenDisk challenge runs,
// the red disk shows; the player walks up, presses E and the disk turns healthy again.
public partial class DiskRepairSpot : Node3D
{
    private const string RepairAction = "repair";

    [Export] public Node3D HealthyDisk { get; set; } = null!;
    [Export] public Node3D FailedDisk { get; set; } = null!;

    private Game _game = null!;
    private InteractionPrompt _prompt = null!;
    private bool _playerInRange;

    public override void _Ready()
    {
        _game = ((Main)GetTree().CurrentScene).Game;
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
        _prompt.Visible = _playerInRange && broken;

        if (_prompt.Visible && Input.IsActionJustPressed(RepairAction))
        {
            ReplaceDisk();
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
