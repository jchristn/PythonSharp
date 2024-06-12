namespace PythonSharp
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Python environment.
    /// </summary>
    public class PythonEnvironment
    {
        #region Public-Members

        /// <summary>
        /// Python executable.
        /// </summary>
        public string PythonExecutable
        {
            get
            {
                return _PythonExecutable;
            }
            set
            {
                if (String.IsNullOrEmpty(value)) throw new ArgumentNullException(nameof(PythonExecutable));
                value = Path.GetFullPath(value);
                if (!File.Exists(value)) throw new FileNotFoundException("The specified Python executable '" + value + "' was not found.");

                _PythonExecutable = value;
            }
        }

        /// <summary>
        /// Script path.
        /// </summary>
        public string ScriptPath
        {
            get
            {
                return _ScriptPath;
            }
            set
            {
                if (!String.IsNullOrEmpty(value)) value = Path.GetFullPath(value);
                else value = "";

                _ScriptPath = value;
            }
        }

        /// <summary>
        /// Virtual environment path.
        /// </summary>
        public string VirtualEnvironmentPath
        {
            get
            {
                return _VirtualEnvironmentPath;
            }
            set
            {
                if (!String.IsNullOrEmpty(value)) value = Path.GetFullPath(value);
                else value = "";

                _VirtualEnvironmentPath = value;
            }
        }

        /// <summary>
        /// Requirements file.
        /// </summary>
        public string RequirementsFile
        {
            get
            {
                return _RequirementsFile;
            }
            set
            {
                if (!String.IsNullOrEmpty(value)) value = Path.GetFullPath(value);
                else value = "";

                _RequirementsFile = value;
            }
        }

        /// <summary>
        /// Separator character for path environment variables.
        /// </summary>
        public string PathSeparator
        {
            get
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return ";";
                else return ":";
            }
        }

        /// <summary>
        /// Separator character for directory paths. 
        /// </summary>
        public string DirectorySeparator
        {
            get
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return "\\";
                else return "/";
            }
        }

        /// <summary>
        /// Shell command for Windows environments.  Default is cmd.exe.
        /// </summary>
        public string ShellCommandWindows
        {
            get
            {
                return _ShellCommandWindows;
            }
            set
            {
                if (String.IsNullOrEmpty(value)) throw new ArgumentNullException(nameof(ShellCommandWindows));
                _ShellCommandWindows = value;
            }
        }

        /// <summary>
        /// Shell command for Linux environments.  Default is bash.
        /// </summary>
        public string ShellCommandLinux
        {
            get
            {
                return _ShellCommandLinux;
            }
            set
            {
                if (String.IsNullOrEmpty(value)) throw new ArgumentNullException(nameof(ShellCommandLinux));
                _ShellCommandLinux = value;
            }
        }

        /// <summary>
        /// Shell command for Mac environments.  Default is bash.
        /// </summary>
        public string ShellCommandMac
        {
            get
            {
                return _ShellCommandMac;
            }
            set
            {
                if (String.IsNullOrEmpty(value)) throw new ArgumentNullException(nameof(ShellCommandMac));
                _ShellCommandMac = value;
            }
        }

        /// <summary>
        /// Boolean indicating whether or not requirements installation via pip should be configured as quiet.
        /// </summary>
        public bool QuietRequirementsInstallation { get; set; } = true;

        /// <summary>
        /// Execution mode.
        /// </summary>
        public ExecutionModeEnum ExecutionMode { get; set; } = ExecutionModeEnum.Filesystem;

        /// <summary>
        /// Boolean indicating whether or not pip packages should be output to the console before running.
        /// </summary>
        public bool DisplayPipPackages { get; set; } = false;

        /// <summary>
        /// Boolean indicating whether or not the generated full script should be output to the console before running.
        /// </summary>
        public bool DisplayFullScript { get; set; } = false;

        #endregion

        #region Private-Members

        private string _PythonExecutable = null;
        private string _ScriptPath = null;
        private string _VirtualEnvironmentPath = null;
        private string _RequirementsFile = null;
        private string _ShellCommandWindows = "cmd.exe";
        private string _ShellCommandLinux = "bash";
        private string _ShellCommandMac = "bash";

        #endregion

        #region Constructors-and-Factories

        /// <summary>
        /// Instantiate.
        /// </summary>
        public PythonEnvironment()
        {

        }

        #endregion

        #region Public-Methods

        #endregion

        #region Private-Methods

        #endregion
    }
}
