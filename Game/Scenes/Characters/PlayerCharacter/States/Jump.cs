using System.Numerics;

namespace MultiplayerPOC.Game.Scenes.Characters.PlayerCharacter.States;

public partial class Jump : PlayerState
{
    private const float JumpVelocity = 7.5f;
    private float _initialHorizontalSpeed;

    public override bool IsInAirState => true;

    public override void Enter(PlayerState previousPlayerState)
    {
        base.Enter(previousPlayerState);

        // Capture horizontal speed at the moment of jumping
        var v = Controller.Velocity;
        var horizontal = new Vector2(v.X, v.Z);
        _initialHorizontalSpeed = horizontal.Length();

        // Apply jump vertical velocity
        v.Y = JumpVelocity;
        Controller.Velocity = v;
    }

    public override void PhysicsUpdate(double delta)
    {
        ApplyGravity(delta);

        var moveDir = Controller.GetMovementDirection(InputDirection);
        var velocity = Controller.Velocity;

        // If there's movement input, apply the initial horizontal speed in that direction.
        // If there's no input, keep the existing horizontal velocity so momentum is preserved.
        if (moveDir.LengthSquared() > 0.0001f)
        {
            velocity.X = moveDir.X * _initialHorizontalSpeed;
            velocity.Z = moveDir.Z * _initialHorizontalSpeed;
        }

        Controller.Velocity = velocity;
        Controller.MoveAndSlide();

        Controller.LookTowardDirection(moveDir, (float)delta);

        // Transition to fall once we're rising slower or start descending
        if (Controller.Velocity.Y <= 0 || !Controller.IsOnFloor()) StateMachine.ChangeState<Fall>();
    }
}