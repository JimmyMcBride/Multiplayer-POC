using Godot;
using MultiplayerPOC.Engine.Core;
using MultiplayerPOC.Game.Globals.Constants;
using MultiplayerPOC.Game.Scenes.Characters.PlayerCharacter.Interfaces;
using MultiplayerPOC.Game.Scenes.Characters.PlayerCharacter.States;

namespace MultiplayerPOC.Game.Scenes.Characters.PlayerCharacter;

[GlobalClass]
public partial class State : Node
{
    private State _previousState;
    protected string DefaultAnimationName = "";

    protected Vector3 RootMotionPosition = Vector3.Zero;
    protected Quaternion RootMotionRotation = Quaternion.Identity;

    protected Vector2 InputDirection => Input.GetVector(
        InputAction.Left,
        InputAction.Right,
        InputAction.Forward,
        InputAction.Backward
    );

    protected StateMachine StateMachine { get; private set; }
    protected ICharacterController Controller { get; private set; }
    public virtual bool IsStateLocked => false;
    protected virtual float DefaultTransitionTime => 0.2f;
    public virtual bool IsInAirState => false;

    protected bool IsActive { get; private set; }

    protected virtual bool CanDash => !IsInAirState && !IsStateLocked;
    protected virtual bool CanSprint => !IsInAirState && !IsStateLocked;
    protected virtual bool CanJump => !IsInAirState;
    protected virtual bool CanPrimaryAttack => !IsStateLocked;
    protected virtual bool CanSecondaryAttack => !IsStateLocked;
    protected virtual bool CanSkillAttack => !IsStateLocked;
    protected virtual bool CanBlock => !IsStateLocked;

    public virtual void Initialize(ICharacterController controller)
    {
        StateMachine = GetParent<StateMachine>();
        Controller = controller;
    }

    public virtual void Enter(State previousState)
    {
        Log.Info($"Entering state '{Name}'");
        IsActive = true;
        _previousState = previousState;
    }

    public virtual void Exit()
    {
        Log.Info($"Exiting state '{Name}'");
        IsActive = false;
    }

    public virtual void SpecialInput(InputEvent @event)
    {
        // Default implementation handles common inputs
        // States can override to customize or add state-specific inputs

        TryAttack();
        TryDash();
        TrySprint();
        TryJump();
    }

    public virtual void Update(double delta)
    {
    }


    public virtual void PhysicsUpdate(double delta)
    {
    }

    protected static Vector2 GetInputDirection()
    {
        return Input.GetVector(InputAction.Left, InputAction.Right, InputAction.Forward, InputAction.Backward);
    }

    protected void ApplyGravity(double delta)
    {
        if (!Controller.Body.IsOnFloor()) Controller.Body.Velocity += Controller.Body.GetGravity() * (float)delta;
    }

    // Input helper methods - check capability flags and handle state transitions
    protected bool TryDash()
    {
        if (!CanDash || !Input.IsActionJustPressed(InputAction.Dash))
            return false;

        // StateMachine.ChangeState<Dash>();
        return true;
    }

    protected bool TrySprint()
    {
        if (!CanSprint || !Input.IsActionJustPressed(InputAction.Sprint))
            return false;

        StateMachine.ChangeState<Sprint>();
        return true;
    }

    protected bool TryJump()
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
    protected bool TryAttack()
    {
        if (IsStateLocked || !Input.IsActionJustPressed(InputAction.Attack))
            return false;


        // Transition to Attack state (handles all attack types)
        // StateMachine.ChangeState<Attack>();
        return true;
    }
}