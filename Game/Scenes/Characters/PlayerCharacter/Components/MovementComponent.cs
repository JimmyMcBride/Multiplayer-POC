using Godot;
using MultiplayerPOC.Engine.Core;

namespace MultiplayerPOC.Game.Scenes.Characters.PlayerCharacter.Components;

public enum FacingMode
{
    Movement,
    Camera
}

public partial class MovementComponent : Node
{
    private Node3D _horizontalPivot;
    private Node3D _pivot;

    public FacingMode CurrentFacingMode { get; private set; } = FacingMode.Movement;

    public void Initialize(
        Node3D pivot,
        Node3D horizontalPivot)
    {
        _pivot = pivot;
        _horizontalPivot = horizontalPivot;
    }

    public void ToggleFacingMode()
    {
        CurrentFacingMode = CurrentFacingMode == FacingMode.Movement
            ? FacingMode.Camera
            : FacingMode.Movement;
        Log.Info($"Facing mode: {CurrentFacingMode}");
    }

    public void LookTowardDirection(Vector3 direction, float delta)
    {
        var lookDirection = CurrentFacingMode == FacingMode.Camera
            ? GetCameraForward()
            : direction;

        if (lookDirection.LengthSquared() < 0.0001f)
            return;

        var targetTransform = _pivot.GlobalTransform.LookingAt(
            _pivot.GlobalPosition + lookDirection,
            Vector3.Up,
            true
        );

        _pivot.GlobalTransform = _pivot.GlobalTransform.InterpolateWith(
            targetTransform,
            1f - Mathf.Exp(-80f * delta)
        );
    }

    public Vector3 GetCameraForward()
    {
        return -_horizontalPivot.GlobalTransform.Basis.Z;
    }

    public Basis GetCharacterBasis()
    {
        return _pivot.GlobalTransform.Basis;
    }

    public Vector3 GetMovementDirection(Vector2 inputDir)
    {
        if (inputDir.LengthSquared() < 0.0001f)
            return Vector3.Zero;

        var inputDir3D = new Vector3(inputDir.X, 0f, inputDir.Y).Normalized();
        return _horizontalPivot.GlobalTransform.Basis * inputDir3D;
    }
}