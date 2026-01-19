using Godot;
using MultiplayerPOC.Game.Globals.Constants;

namespace MultiplayerPOC.Game.Scenes.Characters.PlayerCharacter.Components;

public partial class SmoothCameraArm : SpringArm3D
{
    private float _decay = 20f;

    private Node3D _verticalPivot;

    private static Vector2 InputDirection => Input.GetVector(
        InputAction.Left,
        InputAction.Right,
        InputAction.Forward,
        InputAction.Backward
    );

    public override void _Ready()
    {
        _verticalPivot = GetNode<Node3D>("%VerticalPivot");
    }

    public override void _PhysicsProcess(double delta)
    {
        var dt = (float)delta;

        GlobalTransform = GlobalTransform.InterpolateWith(
            _verticalPivot.GlobalTransform,
            1f - Mathf.Exp(-_decay * dt)
        );
    }
}