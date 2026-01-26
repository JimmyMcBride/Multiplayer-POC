using Godot;
using MultiplayerPOC.Engine.Core;

namespace MultiplayerPOC.Game.Globals;

public partial class NetworkManager : Node
{
    public const int DefaultPort = 7777;

    private ENetMultiplayerPeer _peer;

    public static NetworkManager Instance { get; private set; }

    public bool IsServer => Multiplayer.IsServer();
    public bool IsClient => !Multiplayer.IsServer() && Multiplayer.HasMultiplayerPeer();
    public int LocalPeerId => Multiplayer.GetUniqueId();

    public int GetPing()
    {
        if (_peer == null || !IsClient) return 0;

        // Peer ID 1 is always the server
        var serverPeer = _peer.GetPeer(1);
        if (serverPeer == null) return 0;

        return (int)serverPeer.GetStatistic(ENetPacketPeer.PeerStatistic.RoundTripTime);
    }

    [Signal] public delegate void ServerStartedEventHandler();
    [Signal] public delegate void ClientConnectedEventHandler(long peerId);
    [Signal] public delegate void ClientDisconnectedEventHandler(long peerId);
    [Signal] public delegate void ConnectedToServerEventHandler();
    [Signal] public delegate void ConnectionFailedEventHandler();

    public override void _Ready()
    {
        Instance = this;
        Multiplayer.PeerConnected += OnPeerConnected;
        Multiplayer.PeerDisconnected += OnPeerDisconnected;
        Multiplayer.ConnectedToServer += OnConnectedToServer;
        Multiplayer.ConnectionFailed += OnConnectionFailed;
        Multiplayer.ServerDisconnected += OnServerDisconnected;
    }

    public override void _ExitTree()
    {
        Multiplayer.PeerConnected -= OnPeerConnected;
        Multiplayer.PeerDisconnected -= OnPeerDisconnected;
        Multiplayer.ConnectedToServer -= OnConnectedToServer;
        Multiplayer.ConnectionFailed -= OnConnectionFailed;
        Multiplayer.ServerDisconnected -= OnServerDisconnected;
    }

    public Error Host(int port = DefaultPort)
    {
        _peer = new ENetMultiplayerPeer();
        var error = _peer.CreateServer(port);

        if (error != Error.Ok)
        {
            Log.Error($"Failed to create server: {error}");
            return error;
        }

        Multiplayer.MultiplayerPeer = _peer;
        Log.Info($"Server started on port {port}");
        EmitSignal(SignalName.ServerStarted);
        return Error.Ok;
    }

    public Error Join(string address, int port = DefaultPort)
    {
        _peer = new ENetMultiplayerPeer();
        var error = _peer.CreateClient(address, port);

        if (error != Error.Ok)
        {
            Log.Error($"Failed to connect to {address}:{port}: {error}");
            return error;
        }

        Multiplayer.MultiplayerPeer = _peer;
        Log.Info($"Connecting to {address}:{port}...");
        return Error.Ok;
    }

    public void Disconnect()
    {
        if (_peer == null) return;

        Log.Info("Disconnecting...");
        Multiplayer.MultiplayerPeer = null;
        _peer.Close();
        _peer = null;
    }

    private void OnPeerConnected(long id)
    {
        Log.Info($"Peer connected: {id}");
        EmitSignal(SignalName.ClientConnected, id);
    }

    private void OnPeerDisconnected(long id)
    {
        Log.Info($"Peer disconnected: {id}");
        EmitSignal(SignalName.ClientDisconnected, id);
    }

    private void OnConnectedToServer()
    {
        Log.Info($"Connected to server as peer {LocalPeerId}");
        EmitSignal(SignalName.ConnectedToServer);
    }

    private void OnConnectionFailed()
    {
        Log.Error("Connection to server failed");
        EmitSignal(SignalName.ConnectionFailed);
        _peer = null;
    }

    private void OnServerDisconnected()
    {
        Log.Warning("Server disconnected");
        _peer = null;
    }
}
