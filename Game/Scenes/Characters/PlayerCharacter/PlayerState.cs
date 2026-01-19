using Godot;
using MultiplayerPOC.Engine.Core;
using MultiplayerPOC.Game.Globals.Constants;
using MultiplayerPOC.Game.Scenes.Characters.PlayerCharacter.States;

namespace MultiplayerPOC.Game.Scenes.Characters.PlayerCharacter;

[GlobalClass]
public partial class PlayerState : Node
{
    private PlayerState _previousPlayerState;

    protected static Vector2 InputDirection => Input.GetVector(
        InputAction.Left,
        InputAction.Right,
        InputAction.Forward,
        InputAction.Backward
    );

    protected virtual float DefaultTransitionTime => 0.2f;

    protected bool IsActive { get; private set; }
    protected StateMachine StateMachine { get; private set; }
    protected PlayerCharacter Controller { get; private set; }


    protected virtual bool CanDash => !IsStateLocked;
    protected virtual bool CanSprint => !IsInAirState && !IsStateLocked;
    protected virtual bool CanJump => !IsInAirState && !IsStateLocked;
    protected virtual bool CanAttack => !IsStateLocked;

    public virtual bool IsStateLocked => false;
    public virtual bool IsInAirState => false;

    public virtual void Exit()
    {
        Log.Info($"Exiting state '{Name}'");
        IsActive = false;
    }

    public virtual void Update(double delta)
    {
    }


    public virtual void PhysicsUpdate(double delta)
    {
    }

    public void Initialize(PlayerCharacter controller)
    {
        StateMachine = GetParent<StateMachine>();
        Controller = controller;
    }

    public void SpecialInput(InputEvent @event)
    {
        TryAttack();
        TryDash();
        TrySprint();
        TryJump();
    }

    public virtual void Enter(PlayerState previousPlayerState)
    {
        Log.Info($"Entering state '{Name}'");
        IsActive = true;
        _previousPlayerState = previousPlayerState;
    }

    protected static Vector2 GetInputDirection()
    {
        return Input.GetVector(InputAction.Left, InputAction.Right, InputAction.Forward, InputAction.Backward);
    }

    protected void ApplyGravity(double delta)
    {
        if (!Controller.IsOnFloor()) Controller.Velocity += Controller.GetGravity() * (float)delta;
    }

    // Input helper methods - check capability flags and handle state transitions
    private bool TryDash()
    {
        if (!CanDash || !Input.IsActionJustPressed(InputAction.Dash))
            return false;

        StateMachine.ChangeState<Dash>();
        return true;
    }

    private bool TrySprint()
    {
        if (!CanSprint || !Input.IsActionJustPressed(InputAction.Sprint))
            return false;

        StateMachine.ChangeState<Sprint>();
        return true;
    }

    private bool TryJump()
    {
        if (!CanJump || !Input.IsActionJustPressed(InputAction.Jump))
            return false;

        StateMachine.ChangeState<Jump>();
        return true;
    }

    /// <summary>
    ///     Unified attack trigger that handles all attack inputs (Light/Heavy/Skill).
    ///     Builds attack context based on character state and transitions to Attack state.
    /// </summary>
    private bool TryAttack()
    {
        if (!CanAttack || !Input.IsActionJustPressed(InputAction.Attack))
            return false;


        // Transition to Attack state (handles all attack types)
        // StateMachine.ChangeState<Attack>();
        return true;
    }
}