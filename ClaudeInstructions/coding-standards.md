# Coding Standards

> **Read when:** Writing new code, reviewing code style, or organizing files. Reference for naming conventions and project patterns.

## C# Conventions

### Naming
| Element | Convention | Example |
|---------|------------|---------|
| Classes | PascalCase | `PlayerCharacter`, `StateMachine` |
| Interfaces | IPascalCase | `ICharacterController` |
| Methods | PascalCase | `GetMovementDirection()` |
| Properties | PascalCase | `CurrentState`, `Body` |
| Private fields | _camelCase | `_stateMachine`, `_look` |
| Constants | PascalCase | `MoveSpeed`, `MouseSensitivity` |
| Parameters | camelCase | `delta`, `previousState` |
| Local variables | camelCase | `velocity`, `direction` |

### Namespace Structure
```csharp
MultiplayerPOC.Engine.Core           // Engine utilities
MultiplayerPOC.Engine.Godot          // Godot extensions
MultiplayerPOC.Game.Globals.Constants // Game constants
MultiplayerPOC.Game.Scenes.Characters.PlayerCharacter           // Player root (PlayerState, StateMachine)
MultiplayerPOC.Game.Scenes.Characters.PlayerCharacter.States    // State classes (Idle, Move, Jump, etc.)
MultiplayerPOC.Game.Scenes.Characters.PlayerCharacter.Components // Components (CameraComponent, MovementComponent)
```

Namespaces mirror the folder structure under the project root.

### File Organization
```csharp
using System;           // System namespaces first
using System.Collections.Generic;
using Godot;             // Godot namespace
using MultiplayerPOC.X;  // Project namespaces last

namespace MultiplayerPOC.Folder.Path;

[Attributes]
public partial class ClassName : BaseClass, IInterface
{
    // 1. Constants
    private const float MoveSpeed = 6f;

    // 2. Private fields
    private StateMachine _stateMachine;

    // 3. Properties
    public PlayerState CurrentState { get; private set; }

    // 4. Godot lifecycle methods (_Ready, _Process, etc.)
    public override void _Ready() { }

    // 5. Public methods
    public void Initialize() { }

    // 6. Protected methods
    protected void ApplyGravity() { }

    // 7. Private methods
    private void HelperMethod() { }
}
```

## Godot-Specific Patterns

### Node References
```csharp
// Prefer GetNode<T> over GetNode + cast
var camera = GetNode<CameraComponent>("CameraComponent");

// Use unique names with % for deeply nested nodes
var pivot = GetNode<Node3D>("%VerticalPivot");
```

### Partial Classes
All Godot node scripts must use `partial` for source generators:
```csharp
public partial class PlayerCharacter : CharacterBody3D { }
```

### Export Attributes
```csharp
[Export] public Node DefaultStateNode;  // Exposed in Inspector
[GlobalClass] public partial class PlayerState : Node { }  // Visible in "Add Node" dialog
```

### Input Handling
Use centralized input action constants from `InputAction.cs`:
```csharp
// Good
if (Input.IsActionJustPressed(InputAction.Jump))

// Avoid hardcoded strings
if (Input.IsActionJustPressed("jump"))
```

### Input Direction
Use `Input.GetVector()` for movement:
```csharp
var inputDir = Input.GetVector(
    InputAction.Left,
    InputAction.Right,
    InputAction.Forward,
    InputAction.Backward
);
```

## Common Patterns

### Exponential Lerp (Smooth Interpolation)
Used for framerate-independent smoothing:
```csharp
var t = 1f - Mathf.Exp(-speed * (float)delta);
value = Mathf.Lerp(value, target, t);
// Or for transforms:
transform = transform.InterpolateWith(targetTransform, t);
```

### Initialization Pattern
Components that depend on external references use explicit initialization:
```csharp
public void Initialize(PlayerCharacter controller)
{
    _controller = controller;
    // Setup that requires controller...
}

// Called from parent's _Ready:
public override void _Ready()
{
    var component = GetNode<Component>("Component");
    component.Initialize(this);
}
```

### State Capability Flags
Override boolean properties to control behavior:
```csharp
public override bool IsInAirState => true;  // Disable ground-only actions
public override bool IsStateLocked => true; // Block all transitions
protected override bool CanJump => false;   // Disable jump specifically
```

## File Locations

| Type | Location |
|------|----------|
| New State | `Game/Scenes/Characters/PlayerCharacter/States/` |
| New Component | `Game/Scenes/Characters/PlayerCharacter/Components/` |
| New Constant | `Game/Globals/Constants/` |
| Engine Utility | `Engine/Core/` |
| Godot Extension | `Engine/Godot/` |
| New Level | `Game/Scenes/Levels/` |
| Materials | `Game/Resources/Materials/` |

## Documentation

- Keep code self-documenting through clear naming
- Add XML docs only for non-obvious public APIs
- State classes document their purpose via class name and capability flags
- Log significant state transitions with `Log.Info()`
