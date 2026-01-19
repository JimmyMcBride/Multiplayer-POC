using Godot;
using MultiplayerPOC.Game.Globals.Constants;

namespace MultiplayerPOC.Game.Scenes.Characters.PlayerCharacter.States;

public partial class Sprint : PlayerState
{
    private const float SprintSpeed = 9f;
    private const float AccelerationTime = 10f;

    public override void PhysicsUpdate(double delta)
    {
        // Check for input release or state transitions
        if (!Input.IsActionPressed(InputAction.Sprint) || GetInputDirection() == Vector2.Zero)
        {
            StateMachine.ChangeState<Move>();
            return;
        }

        if (!Controller.IsOnFloor())
        {
            StateMachine.ChangeState<Fall>();
            return;
        }

        var velocity = Controller.Velocity;
        var direction = Controller.GetMovementDirection(InputDirection);

        var targetVelocityX = direction.X * SprintSpeed;
        var targetVelocityZ = direction.Z * SprintSpeed;

        var t = (float)(delta * AccelerationTime);
        velocity.X = Mathf.Lerp(velocity.X, targetVelocityX, t);
        velocity.Z = Mathf.Lerp(velocity.Z, targetVelocityZ, t);

        // Always face movement direction while sprinting, even when locked on
        Controller.LookTowardDirection(direction, (float)delta);

        Controller.Velocity = velocity;
        Controller.MoveAndSlide();
    }
}