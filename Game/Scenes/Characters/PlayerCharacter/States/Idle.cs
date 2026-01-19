using Godot;

namespace MultiplayerPOC.Game.Scenes.Characters.PlayerCharacter.States;

public partial class Idle : PlayerState
{
    public override void PhysicsUpdate(double delta)
    {
        if (!Controller.IsOnFloor())
            StateMachine.ChangeState<Fall>();

        if (InputDirection != Vector2.Zero)
        {
            // Prefer sprint if the sprint action is held while moving
            if (Input.IsActionPressed("sprint"))
                StateMachine.ChangeState<Sprint>();
            else
                StateMachine.ChangeState<Move>();
        }

        var velocity = Controller.Velocity;
        velocity.X = Mathf.MoveToward(Controller.Velocity.X, 0, (float)delta * 25);
        velocity.Z = Mathf.MoveToward(Controller.Velocity.Z, 0, (float)delta * 25);
        Controller.Velocity = velocity;
    }
}