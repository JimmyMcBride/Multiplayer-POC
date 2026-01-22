using Godot;
using MultiplayerPOC.Engine.Core;
using MultiplayerPOC.Game.Globals;

namespace MultiplayerPOC.Game.Scenes.Levels.Main;

public partial class Main : Node3D
{
    private Node3D _playerContainer;
    private Marker3D _playerSpawn;
    private MultiplayerSpawner _spawner;
    [Export] public PackedScene PlayerScene;

    public override void _Ready()
    {
        _playerContainer = GetNode<Node3D>("PlayerContainer");
        _playerSpawn = GetNode<Marker3D>("PlayerSpawn");
        _spawner = GetNode<MultiplayerSpawner>("MultiplayerSpawner");

        // Set custom spawn function so we can configure authority properly
        _spawner.SpawnFunction = new Callable(this, MethodName.SpawnPlayerFromData);

        NetworkManager.Instance.ServerStarted += OnServerStarted;
        NetworkManager.Instance.ClientConnected += OnClientConnected;
        NetworkManager.Instance.ClientDisconnected += OnClientDisconnected;
        NetworkManager.Instance.ConnectedToServer += OnConnectedToServer;

        StartNetworking();
    }

    private void StartNetworking()
    {
        if (GameConfig.Instance.IsServer)
            NetworkManager.Instance.Host(GameConfig.Instance.Port);
        else
            NetworkManager.Instance.Join(
                GameConfig.Instance.ServerAddress,
                GameConfig.Instance.Port);
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
        Log.Info("Server started");
        // Dedicated server has no player - players are spawned when clients connect
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
        // Use spawner.Spawn() with peer ID as data - the spawn function handles instantiation
        _spawner.Spawn(peerId);
        Log.Info($"Spawned player for peer {peerId}");
    }

    // Called by MultiplayerSpawner - returns configured node to be added to spawn_path
    private Node SpawnPlayerFromData(Variant data)
    {
        var peerId = data.AsInt32();
        var player = PlayerScene.Instantiate<CharacterBody3D>();
        player.Name = $"Player_{peerId}";
        player.SetMultiplayerAuthority(peerId);
        player.Position = _playerSpawn.Position;
        return player;
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