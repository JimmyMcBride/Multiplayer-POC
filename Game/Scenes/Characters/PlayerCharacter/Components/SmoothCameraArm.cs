using Godot;
using MultiplayerPOC.Game.Globals.Constants;

namespace MultiplayerPOC.Game.Scenes.Characters.PlayerCharacter.Components;

public partial class SmoothCameraArm : SpringArm3D
{
    private const float MaxHorizontalOffset = 0.5f;
    private const float MaxVerticalOffset = 0.25f;
    private const float OffsetLerpSpeed = 4f;

    private PlayerCharacter _characterController;
    private Vector3 _currentLocalOffset = Vector3.Zero; // x = right, z = forward/back
    private Node3D _verticalPivot;

    private static Vector2 InputDirection => Input.GetVector(
        InputAction.Left,
        InputAction.Right,
        InputAction.Forward,
        InputAction.Backward
    );

    public void Initialize(PlayerCharacter controller)
    {
        _characterController = controller;
    }

    public override void _Ready()
    {
        _verticalPivot = GetNode<Node3D>("%VerticalPivot");

        if (_characterController == null)
        {
            var current = GetParent();
            while (current != null && _characterController == null)
                if (current is PlayerCharacter controller)
                    _characterController = controller;
                else
                    current = current.GetParent();
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        var dt = (float)delta;

        var isAirborne = _characterController?.IsInAir ?? false;

        var desiredLocal = Vector3.Zero;

        if (!isAirborne)
        {
            var input = InputDirection; // X = left/right, Y = forward/back

            desiredLocal.X = -input.X * MaxHorizontalOffset;
            desiredLocal.Z = -input.Y * MaxVerticalOffset;

            desiredLocal.X = Mathf.Clamp(desiredLocal.X, -MaxHorizontalOffset, MaxHorizontalOffset);
            desiredLocal.Z = Mathf.Clamp(desiredLocal.Z, -MaxVerticalOffset, MaxVerticalOffset);
        }

        var offsetT = 1f - Mathf.Exp(-OffsetLerpSpeed * dt);
        _currentLocalOffset = _currentLocalOffset.Lerp(desiredLocal, offsetT);

        var baseTransform = _verticalPivot.GlobalTransform;

        if (_currentLocalOffset.LengthSquared() > 0.000001f)
        {
            var basis = baseTransform.Basis;
            var right = basis.X;
            var forward = basis.Z;

            var worldOffset = right * _currentLocalOffset.X + forward * _currentLocalOffset.Z;
            baseTransform.Origin += worldOffset;
        }

        GlobalTransform = baseTransform;
    }
}