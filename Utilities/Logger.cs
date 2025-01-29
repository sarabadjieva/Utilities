namespace Utilities
{
    public class Logger
    {
        private const string LOG_EXT = ".log";
        // Save to project specific file
        private readonly string _logFile;
        private readonly string _dateTimeFormat = "dd/MM/yyyy HH:mm:ss";
        private object _lock;

        private string Now => $"[{DateTime.Now.ToString(_dateTimeFormat)}]";

        private enum LogLevel
        {
            Debug, Info, Warning, Error
        }

        public Logger()
        {
            // Optimize?
            _logFile = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + LOG_EXT;

            // Log file header line
            if (!File.Exists(_logFile))
            {
                WriteLine(Now + " log file created");
            }
        }

        public void LogDebug(string message) =>
            WriteFormattedMessage(message, LogLevel.Debug);

        public void Log(string message) =>
            WriteFormattedMessage(message, LogLevel.Info);

        public void LogWarning(string message) =>
            WriteFormattedMessage(message, LogLevel.Warning);

        public void LogError(string message) =>
            WriteFormattedMessage(message, LogLevel.Error);

        private void WriteFormattedMessage(string message, LogLevel logLevel)
        {
            message = logLevel switch
            {
                LogLevel.Debug => Now + " DEBUG | " + message,
                LogLevel.Info => Now + " INFO | " + message,
                LogLevel.Warning => Now + " WARNING | " + message,
                LogLevel.Error => Now + " ERROR | " + message,
                _ => Now + message,
            };

            WriteLine(message);
        }

        private void WriteLine(string message)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(message))
                    return;

                lock (_lock)
                {
                    using StreamWriter writer = new(_logFile);
                    writer.WriteLine(message);
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
