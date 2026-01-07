namespace Main.Source.Utilities
{
    public class Logger
    {
        private const string LogFolderName = "_logs";
        private const string LogFileExtention = ".log";

        private static string _logFilePath = string.Empty;

        private enum LogLevel
        {
            Info,
            Warn,
            Error,
        }

        public static void Info(string msg)
        {
            Log(msg, LogLevel.Info);
        }

        public static void Warn(string msg)
        {
            Log(msg, LogLevel.Warn);
        }

        public static void Error(string msg)
        {
            Log(msg, LogLevel.Error);
        }

        private static void Log(string msg, LogLevel level)
        {
            Guard();

            string logDate = DateTime.Now.ToString("HH:mm:ss");

            string logMsg = ConfigMisc.DEBUG_MODE
                ? msg
                : msg;

            string logData = $"[{logDate}] [{level.ToString().ToUpper()}] {logMsg}" + Environment.NewLine;

            File.AppendAllText(_logFilePath, logData);
        }

        private static void Guard()
        {
            string baseDir = ConfigMisc.DEBUG_MODE
                ? Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                : Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            string logFolder = Path.Combine(baseDir, LogFolderName);
            if (!Directory.Exists(logFolder))
            {
                Directory.CreateDirectory(logFolder);
            }

            string logFileName = DateTime.Now.ToString("d-M-yyyy");

            _logFilePath = Path.Combine(logFolder, logFileName + LogFileExtention);
            if (File.Exists(_logFilePath))
            {
                return;
            }

            File.Create(_logFilePath).Dispose();
        }
    }
}
