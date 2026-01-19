namespace MultiplayerPOC.Game.Scenes.Characters.PlayerCharacter.States;

public partial class Jump : State
{
    private const float JumpVelocity = 7.5f;

    public override bool IsInAirState => true;

    public override void Enter(State previousState)
    {
        base.Enter(previousState);

        var body = Controller.Body;

        // Apply jump
        var v = body.Velocity;
        v.Y = JumpVelocity;
        body.Velocity = v;
    }

    public override void PhysicsUpdate(double delta)
    {
        var body = Controller.Body;

        ApplyGravity(delta);

        // Horizontal input while jumping (optional)
        var moveDir = Controller.GetMovementDirection(InputDirection);
        var velocity = body.Velocity;

        // Basic air control
        const float airSpeed = 6f;
        velocity.X = moveDir.X * airSpeed;
        velocity.Z = moveDir.Z * airSpeed;

        body.Velocity = velocity;
        body.MoveAndSlide();

        Controller.LookTowardDirection(moveDir, (float)delta);

        // Transition to fall once we're rising slower or start descending
        if (body.Velocity.Y <= 0 || !body.IsOnFloor()) StateMachine.ChangeState<Fall>();
    }
}