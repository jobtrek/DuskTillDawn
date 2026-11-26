using Godot;

// What a lightning-struck panel gives off until it is repaired: dark smoke, bursts of sparks from
// the burnt wiring, and the short flashes those sparks throw on the roof.
public partial class PanelDamage : Node3D
{
    [Export(PropertyHint.Range, "0,8,0.1")] public float SmoulderEnergy { get; set; } = 1.2f;
    [Export(PropertyHint.Range, "0,8,0.1")] public float SparkLightEnergy { get; set; } = 4f;

    private GpuParticles3D _smoke = null!;
    private GpuParticles3D _sparks = null!;
    private OmniLight3D _sparkLight = null!;
    private readonly RandomNumberGenerator _rng = new();
    private double _nextFlicker;

    public override void _Ready()
    {
        _smoke = GetNode<GpuParticles3D>("Smoke");
        _sparks = GetNode<GpuParticles3D>("Sparks");
        _sparkLight = GetNode<OmniLight3D>("SparkLight");
        Stop();
    }

    public void ShowAt(Vector3 point)
    {
        if (Visible && GlobalPosition.IsEqualApprox(point))
        {
            return;
        }

        GlobalPosition = point;
        Visible = true;
        _smoke.Restart();
        _sparks.Restart();
        _smoke.Emitting = true;
        _sparks.Emitting = true;
    }

    public void Stop()
    {
        Visible = false;
        _smoke.Emitting = false;
        _sparks.Emitting = false;
        _sparkLight.LightEnergy = 0f;
    }

    public override void _Process(double delta)
    {
        if (!Visible)
        {
            return;
        }

        _nextFlicker -= delta;
        if (_nextFlicker > 0)
        {
            return;
        }

        bool spark = _rng.Randf() < 0.35f;
        _sparkLight.LightEnergy = spark
            ? SparkLightEnergy * _rng.RandfRange(0.6f, 1f)
            : SmoulderEnergy * _rng.RandfRange(0.7f, 1f);
        _nextFlicker = spark ? _rng.RandfRange(0.04f, 0.1f) : _rng.RandfRange(0.15f, 0.6f);
    }
}
