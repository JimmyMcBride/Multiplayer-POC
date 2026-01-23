using Godot;
using MultiplayerPOC.Engine.Core;

namespace MultiplayerPOC.Game.Globals;

public enum GameMode
{
    Client,
    Server
}

public partial class GameConfig : Node
{
    public static GameConfig Instance { get; private set; }

    public GameMode Mode { get; private set; } = GameMode.Client;
    public string ServerAddress { get; private set; } = "47.160.28.139";
    public int Port { get; private set; } = 7777;

    public bool IsServer => Mode == GameMode.Server;
    public bool IsClient => Mode == GameMode.Client;

    public override void _Ready()
    {
        Instance = this;
        ParseCommandLineArgs();
        LogConfiguration();
    }

    private void ParseCommandLineArgs()
    {
        var args = OS.GetCmdlineArgs();

        foreach (var arg in args)
            if (arg == "--server")
            {
                Mode = GameMode.Server;
            }
            else if (arg.StartsWith("--address="))
            {
                ServerAddress = arg.Replace("--address=", "");
            }
            else if (arg.StartsWith("--port="))
            {
                var portStr = arg.Replace("--port=", "");
                if (int.TryParse(portStr, out var port))
                    Port = port;
                else
                    Log.Warning($"Invalid port '{portStr}', using default {Port}");
            }
    }

    private void LogConfiguration()
    {
        if (IsServer)
            Log.Info($"Starting as DEDICATED SERVER on port {Port}");
        else
            Log.Info($"Starting as CLIENT, will connect to {ServerAddress}:{Port}");
    }
}