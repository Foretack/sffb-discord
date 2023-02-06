global using Serilog;
using Serilog.Core;
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

_ = Console.ReadLine();
