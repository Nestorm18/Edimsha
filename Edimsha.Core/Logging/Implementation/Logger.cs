using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Edimsha.Core.Logging.Core;

// ReSharper disable ExplicitCallerInfoArgument

namespace Edimsha.Core.Logging.Implementation
{
    public static class Logger
    {
        private static bool _isStarted;
        private static string _fileName = string.Empty;
        private static string _logsPath;
        private static BaseLogFactory _instance;
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
            _sw.WriteLine(_instance.Log(message, level, origin, filePath, lineNumber));
        }

        private static void Start()
        {
            if (_isStarted) return;
            _isStarted = true;

            // Find current app location
            _logsPath = $"{Directory.GetCurrentDirectory()}/logs/";

            // Add log directory if not exists
            if (!Directory.Exists(_logsPath)) Directory.CreateDirectory(_logsPath);

            // The file log name
            var currentTime = DateTime.Now.ToString("dddd-dd-MMMM-yyyy--HH_mm_ss");
            _fileName = $"{_logsPath}/edimsha_{currentTime}.log";

            // Create a file once
            var fs = new FileStream(_fileName, FileMode.OpenOrCreate);
            fs.Close();
        }

        public static void Setup()
        {
            Start();
            GetInstance();

            _sw = new StreamWriter(_fileName, true, Encoding.ASCII);

            Log("App Setup done!");
        }

        public static void Close()
        {
            Log("App Close done!");
            _sw.Close();

            DeleteNonErrorFiles();
        }

        private static void DeleteNonErrorFiles()
        {
            foreach (var file in Directory.EnumerateFiles(_logsPath))
            {
                var lines = File.ReadAllLines(file);

                var flag = lines.Any(line => line.Contains("[Error      ]"));

                if (!flag)
                    File.Delete(file);
            }
        }
    }
}