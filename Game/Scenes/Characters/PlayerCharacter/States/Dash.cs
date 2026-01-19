using Godot;

namespace MultiplayerPOC.Game.Scenes.Characters.PlayerCharacter.States;

public partial class Dash : PlayerState
{
    private const float DashSpeed = 7f;

    private Vector3 _dashDirection;

    public override bool IsStateLocked => true;

    public override void Enter(PlayerState previousPlayerState)
    {
        base.Enter(previousPlayerState);

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
        if (!Controller.IsOnFloor())
        {
            StateMachine.ChangeState<Fall>();
            return;
        }

        // Apply programmatic motion - constant dash speed
        var horizontalVelocity = _dashDirection.Normalized() * DashSpeed;
        Controller.Velocity = new Vector3(
            horizontalVelocity.X,
            Controller.Velocity.Y, // Preserve Y velocity for gravity
            horizontalVelocity.Z
        );

        Controller.MoveAndSlide();
    }
}