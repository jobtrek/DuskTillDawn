using Godot;

// How rain changes the look of the world: dimmer light and a greyer sky, on top of day and night.
public partial class RainLook : Node
{
    // How much of the light a full storm takes away.
    [Export(PropertyHint.Range, "0,1,0.05")] public float MaxLightDim { get; set; } = 0.6f;

    // How grey the sky gets in a full storm.
    [Export(PropertyHint.Range, "0,1,0.05")] public float MaxSkyGrey { get; set; } = 0.7f;

    public float LightFactor(float rain)
    {
        return 1f - rain * MaxLightDim;
    }

    // Greys the sky towards its own brightness, so a dark night sky stays dark and only loses its blue.
    public Color Greyed(Color sky, float rain)
    {
        var grey = new Color(sky.Luminance, sky.Luminance, sky.Luminance);
        return sky.Lerp(grey, rain * MaxSkyGrey);
    }
}
