using DuskTillDawn.Core;
using Godot;

// The storm's lightning: a flicker of cold light, then thunder a moment later (sound travels slower than light).
public partial class Lightning : Node
{
    [Export(PropertyHint.Range, "0,16,0.1")] public float FlashEnergy { get; set; } = 6f;

    // The thunder file starts with rain; the thunder roll begins about here.
    [Export] public float ThunderStartSeconds { get; set; } = 19.5f;
    [Export] public float ThunderSeconds { get; set; } = 9f;

    private const float FadeOutSeconds = 2f;
    private const float MinThunderDelay = 0.3f;
    private const float MaxThunderDelay = 1.5f;

    private DirectionalLight3D _flash = null!;
    private AudioStreamPlayer _thunder = null!;
    private WeatherSystem? _weather;
    private Tween? _flashTween;
    private Tween? _thunderTween;

    public override void _Ready()
    {
        _flash = GetNode<DirectionalLight3D>("Flash");
        _thunder = GetNode<AudioStreamPlayer>("Thunder");
        _flash.LightEnergy = 0f;
    }

    public void Watch(WeatherSystem weather)
    {
        _weather = weather;
        _weather.LightningStruck += OnStrike;
    }

    // A C# event keeps a reference to this node, so stop listening before Godot frees it.
    public override void _ExitTree()
    {
        if (_weather != null)
        {
            _weather.LightningStruck -= OnStrike;
        }
    }

    private void OnStrike()
    {
        Flash();
        float delay = (float)GD.RandRange(MinThunderDelay, MaxThunderDelay);
        _thunderTween?.Kill();
        _thunderTween = CreateTween();
        _thunderTween.TweenInterval(delay);
        _thunderTween.TweenCallback(Callable.From(StartThunder));
        _thunderTween.TweenInterval(ThunderSeconds);
        _thunderTween.TweenProperty(_thunder, "volume_linear", 0f, FadeOutSeconds);
        _thunderTween.TweenCallback(Callable.From(_thunder.Stop));
    }

    // Real lightning flickers: a bright stroke, a dip, a weaker return stroke, then it fades.
    private void Flash()
    {
        _flashTween?.Kill();
        _flashTween = CreateTween();
        _flashTween.TweenProperty(_flash, "light_energy", FlashEnergy, 0.03f);
        _flashTween.TweenProperty(_flash, "light_energy", FlashEnergy * 0.15f, 0.07f);
        _flashTween.TweenProperty(_flash, "light_energy", FlashEnergy * 0.7f, 0.04f);
        _flashTween.TweenProperty(_flash, "light_energy", 0f, 0.4f);
    }

    private void StartThunder()
    {
        _thunder.VolumeLinear = 1f;
        _thunder.Play(ThunderStartSeconds);
    }
}
