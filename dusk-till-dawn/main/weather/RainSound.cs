using DuskTillDawn.Core.Weather;
using Godot;

// Two rain loops that always play together: light rain fades into heavy rain as the storm builds.
public partial class RainSound : Node
{
    // The first drops are quiet: the sound fades in over the start of the rain instead of popping.
    private const float FullVolumeFromRain = 0.2f;

    private AudioStreamPlayer _light = null!;
    private AudioStreamPlayer _heavy = null!;

    public override void _Ready()
    {
        _light = GetNode<AudioStreamPlayer>("Light");
        _heavy = GetNode<AudioStreamPlayer>("Heavy");
        _light.VolumeLinear = 0f;
        _heavy.VolumeLinear = 0f;
        _light.Play();
        _heavy.Play();
    }

    // Equal-power crossfade: the total loudness stays steady while light turns into heavy.
    public void ShowWeather(WeatherSystem weather)
    {
        float rain = weather.RainIntensity;
        float audible = Mathf.Clamp(rain / FullVolumeFromRain, 0f, 1f);
        float angle = rain * Mathf.Pi / 2f;
        _light.VolumeLinear = audible * Mathf.Cos(angle);
        _heavy.VolumeLinear = audible * Mathf.Sin(angle);
    }
}
