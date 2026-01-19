using Godot;

namespace MultiplayerPOC.Game.Scenes.Characters.PlayerCharacter.States;

public partial class Dash : PlayerState
{
    private const float DashSpeed = 40f;

    private Vector3 _dashDirection;
    private float _elapsedSeconds; // timer

    public override bool IsStateLocked => true;

    public override void Enter(PlayerState previousPlayerState)
    {
        base.Enter(previousPlayerState);

        _elapsedSeconds = 0f; // reset timer

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
        // Update timer
        _elapsedSeconds += (float)delta;
        if (_elapsedSeconds >= .5f)
        {
            StateMachine.ChangeState<Idle>(); // exit to idle after 1 second
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