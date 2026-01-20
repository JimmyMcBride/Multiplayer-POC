# Architecture Overview

> **Read when:** Understanding system design, component relationships, or data flow. Useful when planning new features or debugging cross-system issues.

## Project Structure

```
multiplayer-poc/
├── Engine/              # Core utilities (engine-agnostic where possible)
│   ├── Core/            # Logging, common utilities
│   └── Godot/           # Godot-specific extensions
├── Game/                # Game-specific code
│   ├── Globals/         # Constants, singletons, shared data
│   │   └── Constants/   # Input actions, game constants
│   ├── Resources/       # Godot resources (.tres files)
│   │   └── Materials/   # Shader materials
│   └── Scenes/          # Scene scripts and .tscn files
│       ├── Characters/  # Player and NPC characters
│       └── Levels/      # Level scenes
└── Assets/              # External assets (textures, models)
```

## Core Systems

### 1. Player Character System

The player character uses a **component-based architecture** with a **finite state machine** for behavior control.

**Node Hierarchy:**
```
PlayerCharacter (CharacterBody3D)
├── Pivot (Node3D)                    # Character rotation target
├── MovementComponent (Node)          # Handles movement logic
├── CameraComponent (Node3D)          # Camera management
│   ├── HorizontalPivot (Node3D)      # Yaw rotation
│   │   └── VerticalPivot (Node3D)    # Pitch rotation
│   └── SmoothCameraArm (SpringArm3D) # Camera offset & collision
└── StateMachine (Node)               # Behavior state machine
    ├── Idle (PlayerState)
    ├── Move (PlayerState)
    ├── Jump (PlayerState)
    ├── Fall (PlayerState)
    ├── Land (PlayerState)
    ├── Sprint (PlayerState)
    └── Dash (PlayerState)
```

**PlayerCharacter provides:**
- `LookTowardDirection()` - Delegates to MovementComponent
- `GetMovementDirection()` - Delegates to MovementComponent
- `GetCharacterBasis()` - Delegates to MovementComponent
- `Velocity`, `IsOnFloor()`, `MoveAndSlide()` - Inherited from CharacterBody3D
- `IsInAir`, `Pivot` - Properties for camera/state access

### 2. State Machine

Located in `Game/Scenes/Characters/PlayerCharacter/`.

**Key Classes:**
- `StateMachine.cs` - Manages state transitions and delegates lifecycle calls
- `PlayerState.cs` - Base class for all states with common functionality

**State Lifecycle:**
1. `Initialize(controller)` - Called once when StateMachine initializes
2. `Enter(previousPlayerState)` - Called when transitioning into this state
3. `Update(delta)` - Called every frame (`_Process`)
4. `PhysicsUpdate(delta)` - Called every physics tick (`_PhysicsProcess`)
5. `SpecialInput(event)` - Called for unhandled input (`_UnhandledInput`)
6. `Exit()` - Called when transitioning out of this state

**Capability Flags (in PlayerState.cs):**
- `CanDash` - Whether dash input is processed
- `CanSprint` - Whether sprint input is processed
- `CanJump` - Whether jump input is processed
- `CanAttack` - Whether attack input is processed
- `IsInAirState` - Whether state represents airborne behavior
- `IsStateLocked` - Whether state blocks certain transitions

### 3. Camera System

The camera uses a two-pivot system for independent yaw/pitch control:

- **HorizontalPivot** - Rotates around Y-axis (yaw)
- **VerticalPivot** - Rotates around X-axis (pitch), clamped to [-75°, 40°]

**SmoothCameraArm** provides:
- SpringArm3D collision avoidance
- Input-reactive offset (camera shifts opposite to movement direction)
- Smooth interpolation using exponential lerp

### 4. Movement System

`MovementComponent` handles:
- Direction calculation relative to camera orientation
- Character rotation toward movement direction
- Exponential interpolation for smooth rotation

**Key Method:** `GetMovementDirection(inputDir)` transforms 2D input to 3D world direction based on camera facing.

## Data Flow

```
Input → State.SpecialInput() → TryX() methods → StateMachine.ChangeState<T>()
                                                         ↓
                                               State.Exit() → State.Enter()
                                                         ↓
                                               State.PhysicsUpdate()
                                                         ↓
                                Controller.GetMovementDirection() → Body.MoveAndSlide()
```

## Engine Utilities

### Logging (`Engine/Core/Log.cs`)
```csharp
Log.Debug("message");   // Green
Log.Info("message");    // Cyan
Log.Warning("message"); // Yellow
Log.Error("message");   // Red
```

Outputs timestamped, color-coded messages via `GD.PrintRich()`. Subscribe to `Log.OnLog` event for custom handlers.

### Vector Extensions (`Engine/Godot/Extensions.cs`)
```csharp
vector.SetX(value)
vector.SetY(value)
vector.SetZ(value)
vector.SetXy(x, y)
vector.SetXz(x, z)
vector.SetYz(y, z)
```

## Scene Files

| Scene | Path | Description |
|-------|------|-------------|
| Main Level | `Game/Scenes/Levels/Main/main.tscn` | Entry scene with test geometry |
| Player | `Game/Scenes/Characters/PlayerCharacter/player_character.tscn` | Player prefab |
| Camera | `Game/Scenes/Characters/PlayerCharacter/Components/camera_component.tscn` | Camera subsystem |
