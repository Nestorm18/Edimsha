using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Edimsha.Core.Logging.Core;

namespace Edimsha.Core.Logging.Implementation
{
    /// <summary>
    /// The standard log factory for Edimsha
    /// Logs details to the Debug by default
    /// </summary>
    public class BaseLogFactory
    {
        #region Protected Methods

        /// <summary>
        /// The list of loggers in this factory
        /// </summary>
        private readonly List<ILogger> _mLoggers = new();

        /// <summary>
        /// A lock for the logger list to keep it thread-safe
        /// </summary>
        private readonly object _mLoggersLock = new();

        #endregion

        #region Public Properties

        /// <summary>
        /// If true, includes the origin of where the log message was logged from
        /// such as the class name, line number and file name
        /// </summary>
        private bool IncludeLogOriginDetails { get; } = true;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public BaseLogFactory()
        {
            // Add console logger
            AddLogger(new DebugLogger());
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds the specific logger to this factory
        /// </summary>
        /// <param name="logger">The logger</param>
        private void AddLogger(ILogger logger)
        {
            // Log the list so it is thread-safe
            lock (_mLoggersLock)
            {
                // If the logger is not already in the list...
                if (!_mLoggers.Contains(logger))
                    // Add the logger to the list
                    _mLoggers.Add(logger);
            }
        }

        /// <summary>
        /// Logs the specific message to all loggers in this factory.
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="level">The level of the message being logged</param>
        /// <param name="origin">The method/function this message was logged in</param>
        /// <param name="filePath">The code filename that this message was logged from</param>
        /// <param name="lineNumber">The line of code in the filename this message was logged from</param>
        public string Log(string message,
            LogLevel level = LogLevel.Informative,
            [CallerMemberName] string origin = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            // If the user wants to know where the log originated from...
            if (IncludeLogOriginDetails)
                message = $"{DateTime.Now:dd/MM/yyyy HH:mm:ss tt} [{level.ToString().PadRight(11)}] " +
                          $"[{Path.GetFileName(filePath)} > {origin}() > Line {lineNumber}] ===> " +
                          $"{message}";

            // Log to all loggers
            lock (_mLoggersLock)
            {
                _mLoggers.ForEach(logger => logger.Log(message, level));
            }

            // Show in console always
            Console.WriteLine(message);

            return message;
        }

        #endregion
    }
}