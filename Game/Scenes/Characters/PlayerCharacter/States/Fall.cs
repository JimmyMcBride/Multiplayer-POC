namespace MultiplayerPOC.Game.Scenes.Characters.PlayerCharacter.States;

public partial class Fall : PlayerState
{
    public override bool IsInAirState => true;

    public override void PhysicsUpdate(double delta)
    {
        ApplyGravity(delta);

        Controller.MoveAndSlide();

        // Facing logic: keep looking at lock-on target while in air
        var moveDir = Controller.GetMovementDirection(InputDirection);

        if (InputDirection.LengthSquared() > 0.0001f)
            Controller.LookTowardDirection(moveDir, (float)delta);

        // Landed?
        if (Controller.IsOnFloor()) StateMachine.ChangeState<Land>();
    }
}