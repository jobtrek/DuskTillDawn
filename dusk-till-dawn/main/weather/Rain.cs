using DuskTillDawn.Core.Weather;
using Godot;

// The rain only exists around the player: a 30 x 30 m box of drops that travels with the camera.
public partial class Rain : GpuParticles3D
{
    private const float HeightAboveCamera = 15f;

    // The rain blends out slowly and never reaches exactly zero, so below this it counts as stopped.
    private const float NoRainBelow = 0.01f;

    public void ShowWeather(WeatherSystem weather)
    {
        AmountRatio = weather.RainIntensity;
        Emitting = weather.RainIntensity >= NoRainBelow;
    }

    public override void _Process(double delta)
    {
        Camera3D? camera = GetViewport().GetCamera3D();
        if (camera == null)
        {
            return;
        }

        GlobalPosition = camera.GlobalPosition + Vector3.Up * HeightAboveCamera;
    }
}
