using Godot;

namespace MultiplayerPOC.Game.Scenes.Characters.PlayerCharacter.States;

public partial class Dash : State
{
    private const float DashSpeed = 7f;

    private Vector3 _dashDirection;

    public override bool IsStateLocked => true;

    public override void Enter(State previousState)
    {
        base.Enter(previousState);

        var inputDir = InputDirection;

        // Determine dash direction and animation (all use programmatic motion)
        if (inputDir.IsZeroApprox())
            // Standing still - backward dash
            _dashDirection = -Controller.GetCharacterBasis().Z; // Backward (opposite of facing)
        else
            // Moving - forward dash
            _dashDirection = Controller.GetCharacterBasis().Z;
    }

    public override void PhysicsUpdate(double delta)
    {
        // Transition to fall if dashed off a ledge
        if (!Controller.Body.IsOnFloor())
        {
            StateMachine.ChangeState<Fall>();
            return;
        }

        // Apply programmatic motion - constant dash speed
        var horizontalVelocity = _dashDirection.Normalized() * DashSpeed;
        Controller.Body.Velocity = new Vector3(
            horizontalVelocity.X,
            Controller.Body.Velocity.Y, // Preserve Y velocity for gravity
            horizontalVelocity.Z
        );

        Controller.Body.MoveAndSlide();
    }
}