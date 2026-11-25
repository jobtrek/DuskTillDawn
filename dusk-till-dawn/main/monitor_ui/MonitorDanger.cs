using Godot;

public static class MonitorDanger
{
    // 0 = at the good limit, 1 = at or past the bad limit. Drives the colour and bar fill, never the displayed value.
    public static float Danger(double good, double bad, double value)
    {
        return (float)Mathf.Clamp(Mathf.InverseLerp(good, bad, value), 0, 1);
    }
}
