using Godot;
using MultiplayerPOC.Engine.Core;
using MultiplayerPOC.Game.Globals.Constants;
using MultiplayerPOC.Game.Scenes.Characters.PlayerCharacter.Components;

namespace MultiplayerPOC.Game.Scenes.Characters.PlayerCharacter;

public partial class PlayerCharacter : CharacterBody3D
{
    private StateMachine _stateMachine;
    public MovementComponent MovementComponent { get; private set; }
    public Node3D Pivot { get; private set; }
    public CameraComponent CameraComponent { get; private set; }
    public bool IsInAir => _stateMachine?.CurrentState?.IsInAirState ?? false;

    public void LookTowardDirection(Vector3 direction, float delta)
    {
        MovementComponent.LookTowardDirection(direction, delta);
    }

    public Basis GetCharacterBasis()
    {
        return MovementComponent.GetCharacterBasis();
    }

    public Vector3 GetMovementDirection(Vector2 inputDir)
    {
        return MovementComponent.GetMovementDirection(inputDir);
    }

    public override void _Ready()
    {
        CameraComponent = GetNode<CameraComponent>("CameraComponent");
        CameraComponent.Initialize(this);
        MovementComponent = GetNode<MovementComponent>("MovementComponent");
        Pivot = GetNode<Node3D>("Pivot");
        MovementComponent.Initialize(
            Pivot,
            CameraComponent.HorizontalPivot
        );
        _stateMachine = GetNode<StateMachine>("StateMachine");
        _stateMachine.Initialize(this);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (Input.IsActionJustPressed(InputAction.Quit))
            GetTree().Quit();

        if (Input.IsActionJustPressed(InputAction.ToggleFacingMode))
            ToggleFacingMode();
    }

    private void ToggleFacingMode()
    {
        Log.Debug("ToggleFacingMode");
        MovementComponent.ToggleFacingMode();
    }
}