using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace PythonSharp
{
    /// <summary>
    /// Logging settings.
    /// </summary>
    public class LoggingSettings
    {
        #region Public-Members

        /// <summary>
        /// Method to invoke to emit log messages.
        /// </summary>
        public Action<Severity, string> Logger { get; set; } = null;

        /// <summary>
        /// Header to prepend to log messages.
        /// </summary>
        public string LogHeader
        {
            get
            {
                return _LogHeader;
            }
            set
            {
                _LogHeader = value;
            }
        }

        /// <summary>
        /// Boolean to indicate whether or not log messages should be emitted to the console.
        /// </summary>
        public bool ConsoleLogging { get; set; } = true;

        /// <summary>
        /// Boolean to indicate whether or not log messages should be emitted containing shell commands.
        /// </summary>
        public bool LogShellCommands { get; set; } = true;

        /// <summary>
        /// Boolean to indicate whether or not console output should be logged.
        /// </summary>
        public bool LogConsoleOutput { get; set; } = true;

        /// <summary>
        /// Boolean to indicate whether or not error output should be logged.
        /// </summary>
        public bool LogErrorOutput { get; set; } = true;

        #endregion

        #region Private-Members

        private string _LogHeader = "[PythonSharp]";

        #endregion

        #region Constructors-and-Factories

        /// <summary>
        /// Instantiate.
        /// </summary>
        public LoggingSettings()
        {

        }

        #endregion

        #region Internal-Methods

        /// <summary>
        /// Emit a log message.
        /// </summary>
        /// <param name="sev">Severity.</param>
        /// <param name="msg">Message.</param>
        internal void Log(Severity sev, string msg)
        {
            string formatted =
                (!String.IsNullOrEmpty(_LogHeader) ? _LogHeader + " " : "")
                + SeverityToString(sev) + " "
                + msg;

            if (ConsoleLogging) Console.WriteLine(formatted);
            Logger?.Invoke(sev, formatted);
        }

        /// <summary>
        /// Log shell command.
        /// </summary>
        /// <param name="cmd">Command.</param>
        /// <param name="result">Result.</param>
        internal void LogShellCommand(string cmd, int? result = null)
        {
            if (LogShellCommands)
            {
                string msg = "shell command: " + cmd;
                if (result != null) msg += " (result: " + result.Value + ")";
                Log(Severity.Debug, msg);
            }
        }

        #endregion

        #region Private-Methods

        private string SeverityToString(Severity sev)
        {
            switch (sev)
            {
                case Severity.Debug:
                    return "[DEBUG]";
                case Severity.Info:
                    return "[INFO ]";
                case Severity.Warn:
                    return "[WARN ]";
                case Severity.Error:
                    return "[ERROR]";
                case Severity.Alert:
                    return "[ALERT]";
                case Severity.Critical:
                    return "[CRIT ]";
                case Severity.Emergency:
                    return "[EMERG]";
                default:
                    throw new ArgumentException("Unknown severity level '" + sev.ToString() + "' specified.");
            }
        }

        #endregion
    }
}
