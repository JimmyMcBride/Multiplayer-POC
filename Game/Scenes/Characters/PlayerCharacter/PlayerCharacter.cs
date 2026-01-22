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
    public bool IsLocalPlayer { get; private set; }

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

    public override void _EnterTree()
    {
        base._EnterTree();

        // Determine locality from node name (Player_{peerId})
        // Authority is set by server in Main.SpawnPlayer before adding to tree
        var myPeerId = Multiplayer.GetUniqueId();
        IsLocalPlayer = Name == $"Player_{myPeerId}";
    }

    public override void _Ready()
    {
        CameraComponent = GetNode<CameraComponent>("CameraComponent");
        MovementComponent = GetNode<MovementComponent>("MovementComponent");
        Pivot = GetNode<Node3D>("Pivot");
        _stateMachine = GetNode<StateMachine>("StateMachine");

        var myPeerId = Multiplayer.GetUniqueId();
        Log.Info($"PlayerCharacter._Ready: {Name}, IsLocal={IsLocalPlayer}, MyPeerId={myPeerId}");

        if (IsLocalPlayer)
        {
            Log.Info($"Initializing as LOCAL player: {Name}");
            CameraComponent.Initialize(this);
            CameraComponent.MakeCurrent();
            MovementComponent.Initialize(Pivot, CameraComponent.HorizontalPivot);
            _stateMachine.Initialize(this);
        }
        else
        {
            Log.Info($"Initializing as REMOTE player: {Name}");
            CameraComponent.SetProcessInput(false);
            MovementComponent.Initialize(Pivot, null);
            _stateMachine.Initialize(this, isLocalPlayer: false);
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (Input.IsActionJustPressed(InputAction.Quit))
            GetTree().Quit();

        if (!IsLocalPlayer) return;

        if (Input.IsActionJustPressed(InputAction.ToggleFacingMode))
            ToggleFacingMode();
    }

    private void ToggleFacingMode()
    {
        Log.Debug("ToggleFacingMode");
        MovementComponent.ToggleFacingMode();
    }
}