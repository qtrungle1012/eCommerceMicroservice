using Serilog;

namespace SharedLibrarySolution.Logs
{
    public static class LogException
    {
        public static void LogError(Exception ex)
        {
            LogToFile(ex.Message);
            LogToConsole(ex.Message);
            LogToDebugger(ex.Message);
        }
        public static void LogToFile(string message) => Log.Information($"{DateTime.Now}: {message}");

        private static void LogToConsole(string message) => Log.Information($"{DateTime.Now}: {message}");

        private static void LogToDebugger(string message) => Log.Information($"{DateTime.Now}: {message}");
    }
}
