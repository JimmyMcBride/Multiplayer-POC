using Godot;

namespace MultiplayerPOC.Game.Scenes.Characters.PlayerCharacter.States;

public partial class Idle : State
{
    public override void PhysicsUpdate(double delta)
    {
        if (!Controller.Body.IsOnFloor())
            StateMachine.ChangeState<Fall>();

        if (GetInputDirection() != Vector2.Zero)
            StateMachine.ChangeState<Move>();

        var velocity = Controller.Body.Velocity;
        velocity.X = Mathf.MoveToward(Controller.Body.Velocity.X, 0, (float)delta * 25);
        velocity.Z = Mathf.MoveToward(Controller.Body.Velocity.Z, 0, (float)delta * 25);
        Controller.Body.Velocity = velocity;
    }
}