using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Edimsha.Core.Logging.Core;

namespace Edimsha.Core.Logging.Implementation
{
    public static class Logger
    {
        private static bool _isStarted;
        private static BaseLogFactory _instance;
        private static string _fileName = string.Empty;
        private static StreamWriter _sw;

        private static void GetInstance()
        {
            _instance ??= new BaseLogFactory();
        }

        /// <summary>
        /// Logs the specific message to all loggers in this factory
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="level">The level of the message being logged</param>
        /// <param name="origin">The method/function this message was logged in</param>
        /// <param name="filePath">The code filename that this message was logged from</param>
        /// <param name="lineNumber">The line of code in the filename this message was logged from</param>
        public static void Log(
            string message,
            LogLevel level = LogLevel.Informative,
            [CallerMemberName] string origin = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            Start();
            GetInstance();

            _sw = new StreamWriter(_fileName, true, Encoding.ASCII);
            _sw.WriteLine(_instance.Log(message, level, origin, filePath, lineNumber));
            _sw.Close();
        }

        private static void Start()
        {
            if (_isStarted) return;
            _isStarted = true;

            var logsPath = $"{Directory.GetCurrentDirectory()}/logs/";

            if (!Directory.Exists(logsPath)) Directory.CreateDirectory(logsPath);

            var currentTime = DateTime.Now.ToString("dddd-dd-MMMM-yyyy--HH_mm_ss");
            _fileName = $"{logsPath}/edimsha_{currentTime}.log";

            var fs = new FileStream(_fileName, FileMode.OpenOrCreate);
            fs.Close();
        }
    }
}