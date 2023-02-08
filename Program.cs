global using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using SFFBot;
using SFFBot.Config;

LoggingLevelSwitch logSwitch = new();

#if DEBUG
logSwitch.MinimumLevel = Serilog.Events.LogEventLevel.Debug;
#endif

Log.Logger = new LoggerConfiguration()
            .MinimumLevel.ControlledBy(logSwitch)
            .WriteTo.Console(theme: AnsiConsoleTheme.Code)
            .CreateLogger();

ConfigLoader.Initialize();

Redis redis = new();
Bot bot = new(redis);
FilterHandler handler = new(redis.Cache);

while (true)
{
    string? @in = Console.ReadLine();
    if (string.IsNullOrEmpty(@in)) continue;

    if (Enum.TryParse<LogEventLevel>(@in, out var res))
    {
        logSwitch.MinimumLevel = res;
    }
}
