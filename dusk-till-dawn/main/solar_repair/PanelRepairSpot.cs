using System.Collections.Generic;
using System.Linq;
using DuskTillDawn.Core;
using Godot;

// The solar panels on the Company roof. Each lightning hit breaks one more panel: it smokes and
// looks burnt, and the player walks up to that panel and presses E to repair just that one.
public partial class PanelRepairSpot : Node3D
{
    private const string RepairAction = "repair";

    // One roof slot per SolarPanel, in the same order as GameState.SolarPanels (PANEL-01 first).
    [Export] public Node3D[] Slots { get; set; } = [];
    [Export] public Node3D CleanModel { get; set; } = null!;
    [Export] public Node3D DamagedModel { get; set; } = null!;

    private sealed record PanelSlot(Node3D Clean, GeometryInstance3D Damaged, Area3D Zone, PanelDamage Damage);

    private readonly Dictionary<SolarPanel, PanelSlot> _slots = [];
    private Game _game = null!;
    private InteractionPrompt _prompt = null!;
    private LightningBolt _bolt = null!;

    // Soot over a struck panel, so it reads as burnt from the isometric camera.
    private static readonly StandardMaterial3D Scorch = new()
    {
        ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
        Transparency = BaseMaterial3D.TransparencyEnum.Alpha,
        AlbedoColor = new Color(0.04f, 0.02f, 0.01f, 0.85f),
    };

    public override void _Ready()
    {
        var main = (Main)GetTree().CurrentScene;
        _game = main.Game;
        _prompt = GetNode<InteractionPrompt>("InteractionPrompt");
        _bolt = GetNode<LightningBolt>("Bolt");
        _game.ChallengeLaunched += OnChallengeLaunched;

        IReadOnlyList<SolarPanel> panels = main.State.SolarPanels.All;
        if (Slots.Length != panels.Count)
        {
            GD.PushError($"PanelRepairSpot has {Slots.Length} slots for {panels.Count} solar panels.");
        }

        // The roof model mixes clean and broken panels; every slot gets both looks so any panel can
        // break, plus its own repair zone and smoke, since several panels can be broken at once.
        var zoneTemplate = GetNode<Area3D>("Zone");
        var damageTemplate = GetNode<PanelDamage>("Damage");
        for (int i = 0; i < Mathf.Min(Slots.Length, panels.Count); i++)
        {
            _slots[panels[i]] = BuildSlot(Slots[i], zoneTemplate, damageTemplate);
        }

        zoneTemplate.QueueFree();
        damageTemplate.QueueFree();
        foreach (Node3D slot in Slots)
        {
            slot.Visible = false;
        }

        if (!InputMap.HasAction(RepairAction))
        {
            InputMap.AddAction(RepairAction);
            InputMap.ActionAddEvent(RepairAction, new InputEventKey { PhysicalKeycode = Key.E });
        }
    }

    public override void _Process(double delta)
    {
        var broken = _game.Challenges
            .OfType<DamagedSolarPanel>()
            .Where(c => c.State == ChallengeState.Running && c.Target != null)
            .Select(c => c.Target!)
            .ToHashSet();

        SolarPanel? reachable = null;
        float closest = float.MaxValue;
        foreach ((SolarPanel panel, PanelSlot slot) in _slots)
        {
            bool isBroken = broken.Contains(panel);
            slot.Clean.Visible = !isBroken;
            slot.Damaged.Visible = isBroken;
            if (!isBroken)
            {
                slot.Damage.Stop();
                continue;
            }

            slot.Damage.ShowAt(slot.Clean.GlobalPosition);
            Node3D? player = slot.Zone.GetOverlappingBodies().FirstOrDefault(body => body is CharacterBody3D);
            float distance = player?.GlobalPosition.DistanceTo(slot.Clean.GlobalPosition) ?? float.MaxValue;
            if (distance < closest)
            {
                closest = distance;
                reachable = panel;
            }
        }

        _prompt.Visible = reachable != null;
        if (reachable != null && Input.IsActionJustPressed(RepairAction))
        {
            _game.RepairPanel(_game.Players[0], reachable);
        }
    }

    // A C# event keeps a reference to this node, so stop listening before Godot frees it.
    public override void _ExitTree()
    {
        _game.ChallengeLaunched -= OnChallengeLaunched;
    }

    private void OnChallengeLaunched(Challenge challenge)
    {
        if (challenge is DamagedSolarPanel { Target: { } target } && _slots.TryGetValue(target, out PanelSlot? slot))
        {
            _bolt.StrikeAt(slot.Clean.GlobalPosition);
        }
    }

    private PanelSlot BuildSlot(Node3D roofSlot, Area3D zoneTemplate, PanelDamage damageTemplate)
    {
        Node3D clean = CopyOnto(CleanModel, roofSlot);
        var damaged = (GeometryInstance3D)CopyOnto(DamagedModel, roofSlot);
        damaged.MaterialOverlay = Scorch;

        var zone = (Area3D)zoneTemplate.Duplicate();
        AddChild(zone);
        Vector3 at = roofSlot.GlobalPosition;
        zone.GlobalPosition = new Vector3(at.X, GlobalPosition.Y, at.Z);

        var damage = (PanelDamage)damageTemplate.Duplicate();
        AddChild(damage);

        return new PanelSlot(clean, damaged, zone, damage);
    }

    private static Node3D CopyOnto(Node3D model, Node3D slot)
    {
        var copy = (Node3D)model.Duplicate();
        slot.GetParent().AddChild(copy);
        copy.Transform = slot.Transform;
        copy.Visible = false;
        return copy;
    }
}
