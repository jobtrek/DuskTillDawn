using System.Collections.Generic;
using Godot;

// The bolt the player sees when lightning hits something on the roof: a jagged stroke from the
// clouds down to the point it hits, with a fork, a flicker and a burst of light where it lands.
public partial class LightningBolt : Node3D
{
    [Export] public float Height { get; set; } = 30f;
    [Export] public float Width { get; set; } = 0.18f;
    [Export] public float Wander { get; set; } = 1.4f;
    [Export] public int Segments { get; set; } = 14;
    [Export(PropertyHint.Range, "0,32,0.5")] public float ImpactLightEnergy { get; set; } = 10f;

    private readonly RandomNumberGenerator _rng = new();
    private ImmediateMesh _mesh = null!;
    private StandardMaterial3D _material = null!;
    private OmniLight3D _impactLight = null!;
    private Tween? _tween;

    public override void _Ready()
    {
        _mesh = new ImmediateMesh();
        _material = new StandardMaterial3D
        {
            ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
            Transparency = BaseMaterial3D.TransparencyEnum.Alpha,
            CullMode = BaseMaterial3D.CullModeEnum.Disabled,
            AlbedoColor = new Color(0.85f, 0.9f, 1f, 0f),
        };
        AddChild(new MeshInstance3D
        {
            Mesh = _mesh,
            MaterialOverride = _material,
            CastShadow = GeometryInstance3D.ShadowCastingSetting.Off,
            TopLevel = true,
        });
        _impactLight = new OmniLight3D
        {
            LightColor = new Color(0.8f, 0.87f, 1f),
            OmniRange = 7f,
            LightEnergy = 0f,
            TopLevel = true,
        };
        AddChild(_impactLight);
    }

    public void StrikeAt(Vector3 point)
    {
        Vector3 toCamera = GetViewport().GetCamera3D()?.GlobalTransform.Basis.Z ?? Vector3.Back;
        Vector3 top = point + new Vector3(_rng.RandfRange(-4f, 4f), Height, _rng.RandfRange(-4f, 4f));
        List<Vector3> stroke = Jagged(top, point, Segments, Wander);

        int forkAt = _rng.RandiRange(Segments / 4, Segments / 2);
        Vector3 forkEnd = stroke[forkAt] + new Vector3(_rng.RandfRange(-5f, 5f), -_rng.RandfRange(4f, 8f), _rng.RandfRange(-5f, 5f));

        _mesh.ClearSurfaces();
        _mesh.SurfaceBegin(Mesh.PrimitiveType.Triangles);
        AddRibbon(stroke, Width, toCamera);
        AddRibbon(Jagged(stroke[forkAt], forkEnd, 5, Wander * 0.6f), Width * 0.5f, toCamera);
        _mesh.SurfaceEnd();

        _impactLight.GlobalPosition = point + Vector3.Up * 0.5f;
        Flicker();
    }

    // A real stroke flashes, dims, re-strikes along the same channel, then fades.
    private void Flicker()
    {
        _tween?.Kill();
        _tween = CreateTween();
        _tween.TweenProperty(_material, "albedo_color:a", 1f, 0.02f);
        _tween.Parallel().TweenProperty(_impactLight, "light_energy", ImpactLightEnergy, 0.02f);
        _tween.TweenProperty(_material, "albedo_color:a", 0.25f, 0.06f);
        _tween.TweenProperty(_material, "albedo_color:a", 1f, 0.04f);
        _tween.TweenProperty(_material, "albedo_color:a", 0f, 0.35f);
        _tween.Parallel().TweenProperty(_impactLight, "light_energy", 0f, 0.45f);
        _tween.TweenCallback(Callable.From(_mesh.ClearSurfaces));
    }

    private List<Vector3> Jagged(Vector3 from, Vector3 to, int segments, float wander)
    {
        var points = new List<Vector3> { from };
        for (int i = 1; i < segments; i++)
        {
            float t = (float)i / segments;
            float taper = Mathf.Sin(t * Mathf.Pi);
            var offset = new Vector3(_rng.RandfRange(-1f, 1f), 0f, _rng.RandfRange(-1f, 1f)) * wander * taper;
            points.Add(from.Lerp(to, t) + offset);
        }

        points.Add(to);
        return points;
    }

    // Flat strips turned towards the camera, so the bolt reads as a bright line from any angle.
    private void AddRibbon(List<Vector3> points, float width, Vector3 toCamera)
    {
        for (int i = 0; i < points.Count - 1; i++)
        {
            Vector3 a = points[i];
            Vector3 b = points[i + 1];
            Vector3 side = (b - a).Cross(toCamera);
            side = side.LengthSquared() < 1e-6f ? Vector3.Right : side.Normalized();
            side *= width * 0.5f;

            _mesh.SurfaceAddVertex(a - side);
            _mesh.SurfaceAddVertex(a + side);
            _mesh.SurfaceAddVertex(b + side);
            _mesh.SurfaceAddVertex(a - side);
            _mesh.SurfaceAddVertex(b + side);
            _mesh.SurfaceAddVertex(b - side);
        }
    }
}
