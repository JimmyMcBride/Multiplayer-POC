namespace MultiplayerPOC.Game.Scenes.Characters.PlayerCharacter.States;

public partial class Fall : State
{
    public override bool IsInAirState => true;

    public override void PhysicsUpdate(double delta)
    {
        var body = Controller.Body;

        ApplyGravity(delta);

        body.MoveAndSlide();

        // Facing logic: keep looking at lock-on target while in air
        var moveDir = Controller.GetMovementDirection(InputDirection);
        var cam = Controller.CameraComponent;
        if (InputDirection.LengthSquared() > 0.0001f)
            Controller.LookTowardDirection(moveDir, (float)delta);

        // Landed?
        if (body.IsOnFloor()) StateMachine.ChangeState<Land>();
    }
}