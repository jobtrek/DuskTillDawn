using Godot;

/// <summary>
/// Covers its rect with voxel-like blocks that pop away as <see cref="Progress"/> goes 0 → 1.
/// Blocks near the centre go first, with some randomness, so the screen "unfolds" like a map.
/// </summary>
public partial class BlockReveal : Control
{
    [Export] public int Columns = 24;
    [Export] public Color BlockColor = new(0.05f, 0.06f, 0.1f);

    private float _progress = 1f;

    [Export(PropertyHint.Range, "0,1,0.01")]
    public float Progress
    {
        get => _progress;
        set
        {
            _progress = value;
            QueueRedraw();
        }
    }

    public override void _Ready()
    {
        MouseFilter = MouseFilterEnum.Ignore;
        Resized += QueueRedraw;
    }

    public override void _Draw()
    {
        if (_progress >= 1f || Size.X <= 0f)
            return;

        float cell = Size.X / Columns;
        int rows = Mathf.CeilToInt(Size.Y / cell);
        var centre = new Vector2(Columns, rows) / 2f;
        float maxDist = centre.Length();

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < Columns; x++)
            {
                float noise = Hash(x, y);
                float dist = (new Vector2(x + 0.5f, y + 0.5f) - centre).Length() / maxDist;
                float revealAt = dist * 0.6f + noise * 0.4f;
                if (revealAt < _progress)
                    continue;

                var shade = BlockColor.Lightened(noise * 0.12f);
                DrawRect(new Rect2(x * cell, y * cell, cell + 1f, cell + 1f), shade);
            }
        }
    }

    private static float Hash(int x, int y)
    {
        float s = Mathf.Sin(x * 12.9898f + y * 78.233f) * 43758.5453f;
        return s - Mathf.Floor(s);
    }
}
