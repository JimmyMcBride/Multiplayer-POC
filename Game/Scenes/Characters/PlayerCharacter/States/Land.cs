using Godot;
using MultiplayerPOC.Game.Globals.Constants;
using MultiplayerPOC.Game.Scenes.Characters.PlayerCharacter;
using MultiplayerPOC.Game.Scenes.Characters.PlayerCharacter.States;

public partial class Land : State
{
    public override void PhysicsUpdate(double delta)
    {
        if (InputDirection != Vector2.Zero)
            if (Input.IsActionPressed(InputAction.Sprint))
                StateMachine.ChangeState<Sprint>();
            else
                StateMachine.ChangeState<Move>();
        else
            StateMachine.ChangeState<Idle>();
    }
}