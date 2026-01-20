using Godot;
using MultiplayerPOC.Engine.Core;
using MultiplayerPOC.Game.Globals;

namespace MultiplayerPOC.Game.Scenes.Levels.Main;

public partial class Main : Node3D
{
    [Export] public PackedScene PlayerScene;
    [Export] public bool AutoHost = true;

    private Node _playerContainer;
    private Marker3D _playerSpawn;

    public override void _Ready()
    {
        _playerSpawn = GetNode<Marker3D>("PlayerSpawn");
        _playerContainer = GetNode<Node>("PlayerContainer");

        NetworkManager.Instance.ServerStarted += OnServerStarted;
        NetworkManager.Instance.ClientConnected += OnClientConnected;
        NetworkManager.Instance.ClientDisconnected += OnClientDisconnected;
        NetworkManager.Instance.ConnectedToServer += OnConnectedToServer;

        if (AutoHost)
        {
            // Try to host, if port is taken, join instead
            if (NetworkManager.Instance.Host() != Error.Ok)
            {
                Log.Info("Port in use - joining existing server instead");
                NetworkManager.Instance.Join("127.0.0.1");
            }
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        // Press H to host, J to join localhost (for testing)
        if (@event is InputEventKey { Pressed: true } key)
        {
            if (key.Keycode == Key.H && !Multiplayer.HasMultiplayerPeer())
            {
                Log.Info("Manual host requested");
                NetworkManager.Instance.Host();
            }
            else if (key.Keycode == Key.J && !Multiplayer.HasMultiplayerPeer())
            {
                Log.Info("Joining localhost...");
                NetworkManager.Instance.Join("127.0.0.1");
            }
        }
    }

    public override void _ExitTree()
    {
        if (NetworkManager.Instance == null) return;
        NetworkManager.Instance.ServerStarted -= OnServerStarted;
        NetworkManager.Instance.ClientConnected -= OnClientConnected;
        NetworkManager.Instance.ClientDisconnected -= OnClientDisconnected;
        NetworkManager.Instance.ConnectedToServer -= OnConnectedToServer;
    }

    private void OnServerStarted()
    {
        Log.Info("Server started - spawning host player");
        SpawnPlayer(1);
    }

    private void OnClientConnected(long peerId)
    {
        if (!Multiplayer.IsServer()) return;
        Log.Info($"Client {peerId} connected - spawning their player");
        SpawnPlayer((int)peerId);
    }

    private void OnClientDisconnected(long peerId)
    {
        if (!Multiplayer.IsServer()) return;
        Log.Info($"Client {peerId} disconnected - removing their player");
        DespawnPlayer((int)peerId);
    }

    private void OnConnectedToServer()
    {
        Log.Info("Connected to server - waiting for player spawn");
    }

    private void SpawnPlayer(int peerId)
    {
        var player = PlayerScene.Instantiate<CharacterBody3D>();
        player.Name = $"Player_{peerId}";
        player.SetMultiplayerAuthority(peerId);  // Set authority BEFORE adding to tree

        _playerContainer.AddChild(player, true);

        var spawnPosition = _playerSpawn.GlobalPosition;
        spawnPosition.Y += 0.5f;  // Spawn above ground to avoid clipping
        player.GlobalPosition = spawnPosition;

        Log.Info($"Spawned player for peer {peerId}");
    }

    private void DespawnPlayer(int peerId)
    {
        var playerName = $"Player_{peerId}";
        var player = _playerContainer.GetNodeOrNull(playerName);
        if (player == null)
        {
            Log.Warning($"Player {playerName} not found for despawn");
            return;
        }

        player.QueueFree();
        Log.Info($"Despawned player for peer {peerId}");
    }
}
