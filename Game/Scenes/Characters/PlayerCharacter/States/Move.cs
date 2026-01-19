using Godot;

namespace MultiplayerPOC.Game.Scenes.Characters.PlayerCharacter.States;

public partial class Move : State
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

        var body = Controller.Body;

        if (!body.IsOnFloor())
        {
            StateMachine.ChangeState<Fall>();
            return;
        }

        var velocity = body.Velocity;

        // No lock-on: always use standard movement direction
        var direction = Controller.GetMovementDirection(InputDirection);

        var targetVelocityX = direction.X * MoveSpeed;
        var targetVelocityZ = direction.Z * MoveSpeed;

        var t = (float)(delta * AccelerationTime);
        velocity.X = Mathf.Lerp(velocity.X, targetVelocityX, t);
        velocity.Z = Mathf.Lerp(velocity.Z, targetVelocityZ, t);

        // No lock-on: always face movement direction
        Controller.LookTowardDirection(direction, (float)delta);

        body.Velocity = velocity;
        body.MoveAndSlide();
    }
}