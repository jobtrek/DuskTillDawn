using DuskTillDawn.Core;
using Godot;

// The fire in the server room. While a Fire challenge runs, the flames burn on the rack and smoke
// rises; the A and B models of each flame lean opposite ways, so swapping them makes it flicker.
public partial class FireEffect : Node3D
{
    private const double FlickerSeconds = 0.15;

    [Export] public Node3D[] FlamesA { get; set; } = [];
    [Export] public Node3D[] FlamesB { get; set; } = [];

    private Game _game = null!;
    private GpuParticles3D _smoke = null!;
    private double _flickerTimer;
    private bool _showA = true;

    public override void _Ready()
    {
        _game = ((Main)GetTree().CurrentScene).Game;
        _smoke = GetNode<GpuParticles3D>("Smoke");
        ShowFire(burning: false);
    }

    public override void _Process(double delta)
    {
        bool burning = HasFire();
        if (burning)
        {
            _flickerTimer += delta;
            if (_flickerTimer >= FlickerSeconds)
            {
                _flickerTimer = 0;
                _showA = !_showA;
            }
        }

        ShowFire(burning);
    }

    private bool HasFire()
    {
        for (var i = 0; i < _game.Challenges.Count; i++)
        {
            Challenge challenge = _game.Challenges[i];
            if (challenge is Fire && challenge.State == ChallengeState.Running)
            {
                return true;
            }
        }

        return false;
    }

    private void ShowFire(bool burning)
    {
        foreach (Node3D flame in FlamesA)
        {
            flame.Visible = burning && _showA;
        }

        foreach (Node3D flame in FlamesB)
        {
            flame.Visible = burning && !_showA;
        }

        _smoke.Emitting = burning;
    }
}
