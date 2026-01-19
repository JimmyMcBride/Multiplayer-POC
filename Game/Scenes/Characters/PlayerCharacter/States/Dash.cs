using Godot;
using MultiplayerPOC.Game.Scenes.Characters.PlayerCharacter.Components;

namespace MultiplayerPOC.Game.Scenes.Characters.PlayerCharacter.States;

public partial class Dash : PlayerState
{
    private const float DashSpeed = 40f;
    private const float DashDuration = 0.25f;

    private Vector3 _dashDirection;
    private float _elapsedSeconds; // timer

    // New fields to capture/restore pre-dash horizontal velocity when dashing in air
    private Vector3 _preDashHorizontalVelocity;
    private bool _wasInAirDuringDash;

    public override bool IsStateLocked => true;

    public override void Enter(PlayerState previousPlayerState)
    {
        base.Enter(previousPlayerState);

        _elapsedSeconds = 0f; // reset timer

        // Capture whether we started the dash while in air and save horizontal velocity
        _wasInAirDuringDash = !Controller.IsOnFloor();
        if (_wasInAirDuringDash)
            _preDashHorizontalVelocity = new Vector3(Controller.Velocity.X, 0f, Controller.Velocity.Z);

        var basis = Controller.GetCharacterBasis();
        var moveDir = basis.X * -InputDirection.X + basis.Z * -InputDirection.Y;
        var stillCharacterDirection = Controller.IsOnFloor()
            ? -basis.Z
            : basis.Z;
        var stillCameraDirection = Controller.IsOnFloor()
            ? -Controller.MovementComponent.GetCameraForward().Normalized()
            : Controller.MovementComponent.GetCameraForward().Normalized();

        // Determine dash direction from movement input (world-space).
        // Project 2D input onto character basis X and Z so dash follows movement direction.
        if (!InputDirection.IsZeroApprox())
        {
            if (moveDir.IsZeroApprox())
                // Fallback to backward dash if projection yields zero
                _dashDirection = stillCharacterDirection;
            else
                _dashDirection = Controller.MovementComponent.CurrentFacingMode == FacingMode.Camera
                    ? moveDir.Normalized()
                    : basis.Z;
        }
        else
        {
            // No input - dash backward relative to character facing
            _dashDirection = Controller.MovementComponent.CurrentFacingMode == FacingMode.Camera
                ? stillCameraDirection
                : stillCharacterDirection;
        }
    }

    public override void PhysicsUpdate(double delta)
    {
        // Update timer
        _elapsedSeconds += (float)delta;
        if (_elapsedSeconds >= DashDuration)
        {
            // Restore pre-dash horizontal speed if we dashed while in air
            if (_wasInAirDuringDash)
                Controller.Velocity = new Vector3(
                    _preDashHorizontalVelocity.X,
                    Controller.Velocity.Y, // Preserve Y velocity for gravity
                    _preDashHorizontalVelocity.Z
                );

            StateMachine.ChangeState<Idle>();
            return;
        }

        // Rotate character to face opposite dash direction if no movement input and in Camera facing mode
        if (InputDirection.IsZeroApprox() && Controller.MovementComponent.CurrentFacingMode == FacingMode.Camera)
            Controller.LookTowardDirection(-_dashDirection, (float)delta);

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