using Godot;

public partial class Player : CharacterBody3D
{
    [Export]
    public float Speed { get; set; } = 4f;

    [Export]
    public float RunSpeed { get; set; } = 8f;

    [Export]
    public float JumpSpeed { get; set; } = 5f;

    [Export]
    public float Gravity { get; set; } = 14f;

    [Export]
    public float MouseSensitivity { get; set; } = 0.003f;

    private Camera3D _camera = null!;

    public override void _Ready()
    {
        _camera = GetNode<Camera3D>("Camera3D");
        FloorMaxAngle = Mathf.DegToRad(60f); // the stairs are ~50 degrees, default max is 45
        FloorSnapLength = 0.5f;
        var body = GetNodeOrNull<Node3D>("MeshInstance3D");
        if (body != null) body.Visible = false;
        var nose = GetNodeOrNull<Node3D>("Nose");
        if (nose != null) nose.Visible = false;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion motion && Input.MouseMode == Input.MouseModeEnum.Captured)
        {
            RotateY(-motion.Relative.X * MouseSensitivity);
            float pitch = _camera.Rotation.X - motion.Relative.Y * MouseSensitivity;
            _camera.Rotation = new Vector3(Mathf.Clamp(pitch, -1.4f, 1.4f), 0, 0);
        }
        else if (@event is InputEventMouseButton { ButtonIndex: MouseButton.Right } click)
        {
            Input.MouseMode = click.Pressed
                ? Input.MouseModeEnum.Captured
                : Input.MouseModeEnum.Visible;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        float x = 0f;
        float z = 0f;
        if (Input.IsKeyPressed(Key.D) || Input.IsActionPressed("ui_right")) x += 1f;
        if (Input.IsKeyPressed(Key.A) || Input.IsActionPressed("ui_left")) x -= 1f;
        if (Input.IsKeyPressed(Key.S) || Input.IsActionPressed("ui_down")) z += 1f;
        if (Input.IsKeyPressed(Key.W) || Input.IsActionPressed("ui_up")) z -= 1f;

        Vector3 direction = (GlobalBasis * new Vector3(x, 0, z)).Normalized();
        Vector3 velocity = direction * (Input.IsKeyPressed(Key.Shift) ? RunSpeed : Speed);
        velocity.Y = Velocity.Y - Gravity * (float)delta;
        if (IsOnFloor() && Input.IsKeyPressed(Key.Space))
        {
            velocity.Y = JumpSpeed;
        }

        Velocity = velocity;
        MoveAndSlide();
    }
}
