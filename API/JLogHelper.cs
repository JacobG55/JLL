using BepInEx.Logging;

namespace JLL.API
{
    public class JLogHelper
    {
        public static JLogLevel LogLevel { get; private set; }

        internal static void UpdateLogLevel()
        {
            LogLevel = JLL.loggingLevel.Value;

            GetSource().LogInfo($"JLL Logging Level set to: {LogLevel}");
        }

        public static ManualLogSource GetSource()
        {
            return JLL.Instance.mls;
        }

        public static bool AcceptableLogLevel(JLogLevel logLevel)
        {
            return (int)logLevel <= (int)LogLevel;
        }

        public static void LogInfo(string message, JLogLevel level = JLogLevel.Debuging)
        {
            if (AcceptableLogLevel(level))
            {
                GetSource().LogInfo("[" + level + "] " + message);
            }
        }
        public static void LogWarning(string message, JLogLevel level = JLogLevel.User)
        {
            if (AcceptableLogLevel(level))
            {
                GetSource().LogWarning("[" + level + "] " + message);
            }
        }
        public static void LogError(string message, JLogLevel level = JLogLevel.User)
        {
            if (AcceptableLogLevel(level))
            {
                GetSource().LogError("[" + level + "] " + message);
            }
        }
        public static void LogFatal(string message, JLogLevel level = JLogLevel.ImportantOnly)
        {
            if (AcceptableLogLevel(level))
            {
                GetSource().LogFatal("[" + level + "] " + message);
            }
        }
    }

    public enum JLogLevel
    {
        ImportantOnly,
        User,
        Debuging,
        Wesley,
    }
}
