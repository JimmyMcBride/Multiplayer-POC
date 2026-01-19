using Godot;
using MultiplayerPOC.Engine.Godot;
using MultiplayerPOC.Game.Globals.Constants;

namespace MultiplayerPOC.Game.Scenes.Characters.PlayerCharacter.Components;

public partial class CameraComponent : Node3D
{
    private const float MouseSensitivity = 0.005f;
    private const float MinBoundary = -75;
    private const float MaxBoundary = 40;

    private SmoothCameraArm _cameraArm;
    private Vector2 _look = Vector2.Zero;

    public Node3D HorizontalPivot { get; private set; }
    public Node3D VerticalPivot { get; private set; }

    public override void _Ready()
    {
        HorizontalPivot = GetNode<Node3D>("HorizontalPivot");
        VerticalPivot = HorizontalPivot.GetNode<Node3D>("VerticalPivot");
        _cameraArm = GetNode<SmoothCameraArm>("SmoothCameraArm");
    }

    public override void _PhysicsProcess(double delta)
    {
        FrameCameraRotation();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (Input.IsActionJustPressed(InputAction.Pause) || Input.IsActionJustPressed(InputAction.Cancel))
            Input.MouseMode = Input.MouseMode == Input.MouseModeEnum.Captured
                ? Input.MouseModeEnum.Visible
                : Input.MouseModeEnum.Captured;

        if (@event is InputEventMouseButton)
            if (Input.MouseMode != Input.MouseModeEnum.Captured)
                Input.MouseMode = Input.MouseModeEnum.Captured;

        if (Input.MouseMode == Input.MouseModeEnum.Captured)
            if (@event is InputEventMouseMotion mouseMotionEvent)
                _look = -mouseMotionEvent.Relative * MouseSensitivity;
    }

    private void FrameCameraRotation()
    {
        HorizontalPivot.RotateY(_look.X);
        VerticalPivot.RotateX(_look.Y);
        VerticalPivot.Rotation = VerticalPivot.Rotation.SetX(Mathf.Clamp(VerticalPivot.Rotation.X,
            Mathf.DegToRad(MinBoundary), Mathf.DegToRad(MaxBoundary)));
        _look = Vector2.Zero;
    }
}