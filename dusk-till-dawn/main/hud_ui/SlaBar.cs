using Godot;

/// <summary>
/// Segmented SLA bar: one voxel block per 10 % (Blocks = 10), coloured by health.
/// The block being drained empties from the top down.
/// </summary>
public partial class SlaBar : Control
{
    [Export] public int Blocks = 10;
    [Export] public int Gap = 2;
    [Export] public double WarnBelow = 60;
    [Export] public double BadBelow = 30;
    [Export] public Color EmptyColor = new(0.14f, 0.15f, 0.2f);
    [Export] public Color GoodColor = new(0.36f, 0.8f, 0.42f);
    [Export] public Color WarnColor = new(0.95f, 0.7f, 0.25f);
    [Export] public Color BadColor = new(0.9f, 0.3f, 0.28f);

    private double _value = 100;

    public double Value
    {
        get => _value;
        set
        {
            double clamped = Mathf.Clamp(value, 0, 100);
            if (clamped == _value)
            {
                return;
            }

            _value = clamped;
            QueueRedraw();
        }
    }

    public Color CurrentColor => _value < BadBelow ? BadColor : _value < WarnBelow ? WarnColor : GoodColor;

    public override void _Ready()
    {
        Resized += QueueRedraw;
    }

    public override void _Draw()
    {
        // Whole pixels only, so the blocks stay crisp.
        int width = Mathf.FloorToInt((Size.X - Gap * (Blocks - 1)) / Blocks);
        int height = Mathf.FloorToInt(Size.Y);
        int edge = Mathf.Max(2, height / 5);
        double perBlock = 100.0 / Blocks;
        Color color = CurrentColor;

        for (int i = 0; i < Blocks; i++)
        {
            var position = new Vector2(i * (width + Gap), 0);
            DrawRect(new Rect2(position, new Vector2(width, height)), EmptyColor);

            double fill = Mathf.Clamp((_value - i * perBlock) / perBlock, 0, 1);
            int filled = Mathf.RoundToInt(fill * height);
            if (filled <= 0)
            {
                continue;
            }

            var top = position + new Vector2(0, height - filled);
            DrawRect(new Rect2(top, new Vector2(width, filled)), color);
            DrawRect(new Rect2(top, new Vector2(width, Mathf.Min(edge, filled))), color.Lightened(0.3f));
            DrawRect(new Rect2(position + new Vector2(0, height - edge), new Vector2(width, edge)), color.Darkened(0.3f));
        }
    }
}
