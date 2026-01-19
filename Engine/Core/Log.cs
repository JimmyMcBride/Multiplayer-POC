using System;
using Godot;

namespace MultiplayerPOC.Engine.Core;

public partial class Log : Node
{
    public delegate void LogEventHandler(string richMessage);

    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error
    }

    public static event LogEventHandler OnLog;

    private static void Logger(string message, LogLevel level = LogLevel.Info)
    {
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        var color = level switch
        {
            LogLevel.Debug => "green",
            LogLevel.Info => "cyan",
            LogLevel.Warning => "yellow",
            LogLevel.Error => "red",
            _ => "white"
        };

        var richMessage = $"[color={color}][{timestamp}] [{level}] {message}[/color]";

        GD.PrintRich(richMessage);

        OnLog?.Invoke(richMessage);
    }

    public static void Debug(string message)
    {
        Logger(message, LogLevel.Debug);
    }

    public static void Info(string message)
    {
        Logger(message);
    }

    public static void Warning(string message)
    {
        Logger(message, LogLevel.Warning);
    }

    public static void Error(string message)
    {
        Logger(message, LogLevel.Error);
    }
}