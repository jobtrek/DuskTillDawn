using Godot;

public partial class Player : CharacterBody3D
{
    private const float CameraYawDegrees = 45f;

    [Export]
    public float Speed { get; set; } = 4f;

    public override void _PhysicsProcess(double delta)
    {
        Vector2 input = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
        Vector3 direction = new Vector3(input.X, 0, input.Y)
            .Rotated(Vector3.Up, Mathf.DegToRad(CameraYawDegrees));

        Velocity = direction * Speed;
        MoveAndSlide();
    }
}
