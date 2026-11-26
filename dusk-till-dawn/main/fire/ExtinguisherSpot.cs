using System.Collections.Generic;
using DuskTillDawn.Core.Incidents;
using DuskTillDawn.Core.Night;
using Godot;

// The extinguisher on the server room wall. The player picks it up with E, carries it in the right
// hand, walks to the fire and presses E again to put it out. Q hangs it back on its bracket.
public partial class ExtinguisherSpot : Node3D
{
    private const string RepairAction = "repair";
    private const string DropAction = "drop";
    private const float FireReach = 2.5f;
    // Pulls the held extinguisher towards the camera so it never pokes through a wall.
    private const float HeldZClipScale = 0.4f;

    [Export] public Node3D Extinguisher { get; set; } = null!;
    [Export] public Node3D FireSpot { get; set; } = null!;

    private Game _game = null!;
    private InteractionPrompt _prompt = null!;
    private GpuParticles3D _spray = null!;
    private Player? _player;
    private bool _playerAtBracket;
    private Node _bracket = null!;
    private Transform3D _onBracket;
    private readonly List<(MeshInstance3D Mesh, int Surface, Material? Original)> _heldMaterials = [];

    public override void _Ready()
    {
        _game = ((Main)GetTree().CurrentScene).Game;
        _prompt = GetNode<InteractionPrompt>("InteractionPrompt");
        _bracket = Extinguisher.GetParent();
        _onBracket = Extinguisher.Transform;

        // The spray comes out of the nozzle, so it travels with the extinguisher into the player's hand.
        _spray = GetNode<GpuParticles3D>("Spray");
        _spray.Reparent(Extinguisher, keepGlobalTransform: false);

        var zone = GetNode<Area3D>("Zone");
        zone.BodyEntered += OnBodyEntered;
        zone.BodyExited += OnBodyExited;

        AddKey(RepairAction, Key.E);
        AddKey(DropAction, Key.Q);
    }

    public override void _Process(double delta)
    {
        PlayerProfile profile = _game.Players[0];
        bool holding = profile.Holding == HeldItem.Extinguisher;
        bool canPickUp = _playerAtBracket && profile.Holding == HeldItem.None;
        bool canUse = holding && _game.IsRunning<Fire>() && IsPlayerNearFire();

        _prompt.Visible = canPickUp || canUse;
        if (canPickUp)
        {
            _prompt.SetText("Press E to pick up the extinguisher");
        }
        else if (canUse)
        {
            _prompt.SetText("Press E to use the extinguisher");
        }

        if (Input.IsActionJustPressed(RepairAction))
        {
            if (canPickUp)
            {
                PickUp(profile);
            }
            else if (canUse)
            {
                UseExtinguisher(profile);
            }
        }
        else if (holding && Input.IsActionJustPressed(DropAction))
        {
            PutBack(profile);
        }
    }

    private static void AddKey(string action, Key key)
    {
        if (!InputMap.HasAction(action))
        {
            InputMap.AddAction(action);
            InputMap.ActionAddEvent(action, new InputEventKey { PhysicalKeycode = key });
        }
    }

    // The zone also overlaps the wall and the floor; only the player counts.
    private void OnBodyEntered(Node3D body)
    {
        if (body is Player player)
        {
            _player = player;
            _playerAtBracket = true;
        }
    }

    private void OnBodyExited(Node3D body)
    {
        if (body is Player)
        {
            _playerAtBracket = false;
        }
    }

    private void PickUp(PlayerProfile profile)
    {
        if (_player == null || !profile.TryPickUp(HeldItem.Extinguisher))
        {
            return;
        }

        Extinguisher.Reparent(_player.RightHand, keepGlobalTransform: false);
        Extinguisher.Transform = Transform3D.Identity;
        DrawInFront();
    }

    private void PutBack(PlayerProfile profile)
    {
        profile.Drop();
        Extinguisher.Reparent(_bracket, keepGlobalTransform: false);
        Extinguisher.Transform = _onBracket;
        RestoreMaterials();
    }

    private void DrawInFront()
    {
        _heldMaterials.Clear();
        foreach (MeshInstance3D mesh in MeshesOf(Extinguisher))
        {
            for (var i = 0; i < mesh.GetSurfaceOverrideMaterialCount(); i++)
            {
                if (mesh.GetActiveMaterial(i)?.Duplicate() is not BaseMaterial3D held)
                {
                    continue;
                }

                held.UseZClipScale = true;
                held.ZClipScale = HeldZClipScale;
                _heldMaterials.Add((mesh, i, mesh.GetSurfaceOverrideMaterial(i)));
                mesh.SetSurfaceOverrideMaterial(i, held);
            }
        }
    }

    private void RestoreMaterials()
    {
        foreach ((MeshInstance3D mesh, int surface, Material? original) in _heldMaterials)
        {
            mesh.SetSurfaceOverrideMaterial(surface, original);
        }

        _heldMaterials.Clear();
    }


    // this basically walks the node tree, all the childrend and their children and hands it back
    // every MeshInstance3D it finds, one at a time, yield return basically lets callers foreachover over "mesh"
    private static IEnumerable<MeshInstance3D> MeshesOf(Node node)
    {
        if (node is MeshInstance3D mesh)
        {
            yield return mesh;
        }

        foreach (Node child in node.GetChildren())
        {
            foreach (MeshInstance3D childMesh in MeshesOf(child))
            {
                yield return childMesh;
            }
        }
    }

    private bool IsPlayerNearFire()
    {
        if (_player == null)
        {
            return false;
        }

        Vector3 offset = _player.GlobalPosition - FireSpot.GlobalPosition;
        offset.Y = 0;
        return offset.Length() <= FireReach;
    }

    // One spray puts out one fire; with several burning, the player has to use it again.
    private void UseExtinguisher(PlayerProfile profile)
    {
        _spray.Restart();
        _game.RepairOne(profile, PlayerAction.UseExtinguisher);
    }
}
