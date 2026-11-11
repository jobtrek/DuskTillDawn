using Godot;

[Tool]
public partial class L1StripLightOn : MeshInstance3D
{
    private const string LightNodeName = "StripOmniLight";

    private Color _lightColor = new Color(1f, 0.95f, 0.85f);
    private float _emissionEnergy = 4f;
    private float _lightEnergy = 2f;
    private float _lightRange = 6f;

    [Export] public Color LightColor { get => _lightColor; set { _lightColor = value; Apply(); } }
    [Export] public float EmissionEnergy { get => _emissionEnergy; set { _emissionEnergy = value; Apply(); } }
    [Export] public float LightEnergy { get => _lightEnergy; set { _lightEnergy = value; Apply(); } }
    [Export] public float LightRange { get => _lightRange; set { _lightRange = value; Apply(); } }

    public override void _Ready() => Apply();

    private void Apply()
    {
        if (!IsInsideTree()) return;

        // 1. Glowing surface (unique material so other meshes aren't affected)
        MaterialOverride = new StandardMaterial3D
        {
            AlbedoColor = LightColor,
            EmissionEnabled = true,
            Emission = LightColor,
            EmissionEnergyMultiplier = EmissionEnergy,
        };

        // 2. Real light. Created without an Owner, so it is visible in the
        //    editor viewport but never saved into main.tscn (no duplicates).
        var light = GetNodeOrNull<OmniLight3D>(LightNodeName);
        if (light == null)
        {
            light = new OmniLight3D { Name = LightNodeName };
            AddChild(light);
        }
        light.LightColor = LightColor;
        light.LightEnergy = LightEnergy;
        light.OmniRange = LightRange;
        light.ShadowEnabled = true;

        // 3. Bloom (the "shine"): needs a WorldEnvironment glow
        var env = GetViewport()?.World3D?.Environment
            ?? (GetTree().Root.FindChild("WorldEnvironment", true, false) as WorldEnvironment)?.Environment;
        if (env != null)
            env.GlowEnabled = true;
    }
}
