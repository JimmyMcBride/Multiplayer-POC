# State Machine Guide

> **Read when:** Creating new character states, modifying state transitions, or debugging state behavior.

## Overview

The character uses a finite state machine where each state is a Godot Node that handles behavior for a specific action (Idle, Move, Jump, etc.).

**Key Files:**
- `Game/Scenes/Characters/PlayerCharacter/StateMachine.cs` - State management
- `Game/Scenes/Characters/PlayerCharacter/State.cs` - Base state class
- `Game/Scenes/Characters/PlayerCharacter/States/*.cs` - Individual states

## Creating a New State

### 1. Create the State Class

Create a new file in `Game/Scenes/Characters/PlayerCharacter/States/`:

```csharp
namespace MultiplayerPOC.Game.Scenes.Characters.PlayerCharacter.States;

public partial class MyNewState : State
{
    // Optional: Define state-specific constants
    private const float SomeSpeed = 5f;

    // Optional: Override capability flags
    public override bool IsInAirState => false;
    public override bool IsStateLocked => false;
    protected override bool CanJump => true;
    protected override bool CanDash => true;
    protected override bool CanSprint => true;
    protected override bool CanAttack => true;

    public override void Enter(State previousState)
    {
        base.Enter(previousState);  // Always call base (handles logging, IsActive)
        // State entry logic...
    }

    public override void Exit()
    {
        base.Exit();  // Always call base
        // Cleanup logic...
    }

    public override void PhysicsUpdate(double delta)
    {
        var body = Controller.Body;

        // 1. Check exit conditions
        if (ShouldExitCondition())
        {
            StateMachine.ChangeState<OtherState>();
            return;
        }

        // 2. Apply physics
        ApplyGravity(delta);  // If needed

        // 3. Calculate movement
        var direction = Controller.GetMovementDirection(InputDirection);
        var velocity = body.Velocity;
        // ... modify velocity ...

        // 4. Apply and move
        body.Velocity = velocity;
        body.MoveAndSlide();

        // 5. Update visuals
        Controller.LookTowardDirection(direction, (float)delta);
    }
}
```

### 2. Add to Scene Tree

In `player_character.tscn`, add the new state as a child of StateMachine:
```
StateMachine
├── Idle
├── Move
├── ... existing states ...
└── MyNewState   ← Add here
```

The StateMachine automatically discovers all State children during `Initialize()`.

### 3. Add Transitions

**From other states to your new state:**
```csharp
// In another state's PhysicsUpdate or SpecialInput:
if (triggerCondition)
{
    StateMachine.ChangeState<MyNewState>();
    return;
}
```

**From your state to others:**
```csharp
// In MyNewState's PhysicsUpdate:
if (exitCondition)
{
    StateMachine.ChangeState<Idle>();
    return;
}
```

## State Base Class Reference

### Available Properties
```csharp
protected StateMachine StateMachine;           // Parent state machine
protected ICharacterController Controller;     // Character controller interface
protected bool IsActive;                       // True while state is current
protected Vector2 InputDirection;              // Current WASD input

// Capability flags (override to customize)
public virtual bool IsStateLocked => false;
public virtual bool IsInAirState => false;
protected virtual bool CanDash => !IsInAirState && !IsStateLocked;
protected virtual bool CanSprint => !IsInAirState && !IsStateLocked;
protected virtual bool CanJump => !IsInAirState;
protected virtual bool CanAttack => !IsStateLocked;
```

### Available Methods
```csharp
// Input helpers (check capability and trigger transitions)
protected bool TryDash();    // Returns true if transitioned
protected bool TryJump();
protected bool TrySprint();
protected bool TryAttack();

// Physics helpers
protected void ApplyGravity(double delta);  // Adds gravity if not on floor

// Input
protected static Vector2 GetInputDirection();  // Static version of InputDirection
```

### Lifecycle Methods to Override
```csharp
public virtual void Initialize(ICharacterController controller)  // One-time setup
public virtual void Enter(State previousState)    // State activated
public virtual void Exit()                        // State deactivated
public virtual void Update(double delta)          // Every frame
public virtual void PhysicsUpdate(double delta)   // Every physics tick
public virtual void SpecialInput(InputEvent @event) // Unhandled input
```

## Common Patterns

### Ground Check → Fall Transition
```csharp
if (!body.IsOnFloor())
{
    StateMachine.ChangeState<Fall>();
    return;
}
```

### Input → Idle Transition
```csharp
if (GetInputDirection() == Vector2.Zero)
{
    StateMachine.ChangeState<Idle>();
    return;
}
```

### Timed State (e.g., Landing)
```csharp
public partial class Land : State
{
    private double _timer;
    private const double Duration = 0.2;

    public override void Enter(State previousState)
    {
        base.Enter(previousState);
        _timer = Duration;
    }

    public override void PhysicsUpdate(double delta)
    {
        _timer -= delta;
        if (_timer <= 0 || GetInputDirection() != Vector2.Zero)
        {
            StateMachine.ChangeState<Idle>();
            return;
        }
    }
}
```

### Locked State (No Interruptions)
```csharp
public partial class SpecialMove : State
{
    public override bool IsStateLocked => true;  // Blocks dash, attack

    public override void SpecialInput(InputEvent @event)
    {
        // Don't call base - ignore all common inputs
    }
}
```

## Transition Flow

```
┌─────────┐ no input  ┌──────┐
│  Idle   │◄─────────┤ Move │
└────┬────┘           └──┬───┘
     │ input              │ no floor
     └────────►Move       ▼
               │      ┌──────┐
     ┌─────────┼──────┤ Fall │
     │         │      └──┬───┘
     │         │         │ landed
     │    space│         ▼
     │         │      ┌──────┐
     │         └─────►│ Land │──► Idle
     │                └──────┘
     │ space
     ▼
┌─────────┐
│  Jump   │──► Fall (when velocity.Y <= 0)
└─────────┘
```

## Debugging States

Use `Log.Info()` for state debugging - the base class already logs Enter/Exit:
```
[INFO] Entering state 'Jump'
[INFO] Exiting state 'Jump'
[INFO] Entering state 'Fall'
```

Check current state:
```csharp
var currentState = _stateMachine.CurrentState;
Log.Debug($"Current: {currentState.Name}, InAir: {currentState.IsInAirState}");
```
