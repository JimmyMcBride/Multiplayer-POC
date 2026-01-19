using Godot;

namespace MultiplayerPOC.Game.Scenes.Characters.PlayerCharacter.States;

public partial class Move : PlayerState
{
    private const float MoveSpeed = 6f;
    private const float AccelerationTime = 10f;

    public override void PhysicsUpdate(double delta)
    {
        if (GetInputDirection() == Vector2.Zero)
        {
            StateMachine.ChangeState<Idle>();
            return;
        }

        if (!Controller.IsOnFloor())
        {
            StateMachine.ChangeState<Fall>();
            return;
        }

        var velocity = Controller.Velocity;

        // No lock-on: always use standard movement direction
        var direction = Controller.GetMovementDirection(InputDirection);

        var targetVelocityX = direction.X * MoveSpeed;
        var targetVelocityZ = direction.Z * MoveSpeed;

        var t = (float)(delta * AccelerationTime);
        velocity.X = Mathf.Lerp(velocity.X, targetVelocityX, t);
        velocity.Z = Mathf.Lerp(velocity.Z, targetVelocityZ, t);

        Controller.LookTowardDirection(direction, (float)delta);

        Controller.Velocity = velocity;
        Controller.MoveAndSlide();
    }
}