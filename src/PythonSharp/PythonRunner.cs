namespace PythonSharp
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;
    using HeyShelli;
    using RestWrapper;

    /// <summary>
    /// Python runner.  
    /// Ensure both uvicorn and fastapi are installed via pip install.
    /// </summary>
    public class PythonRunner
    {
        #region Public-Members

        /// <summary>
        /// Logging settings.
        /// </summary>
        public LoggingSettings Logging
        {
            get
            {
                return _Logging;
            }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(Logging));
                _Logging = value;
            }
        }

        /// <summary>
        /// Python environment.
        /// </summary>
        public PythonEnvironment PythonEnvironment
        {
            get
            {
                return _PythonEnvironment;
            }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(PythonEnvironment));
                _PythonEnvironment = value;
            }
        }

        #endregion

        #region Private-Members

        private LoggingSettings _Logging = new LoggingSettings();
        private PythonEnvironment _PythonEnvironment = new PythonEnvironment();

        #endregion

        #region Constructors-and-Factories

        /// <summary>
        /// Instantiate.
        /// </summary>
        public PythonRunner(PythonEnvironment env)
        {
            if (env == null) throw new ArgumentNullException(nameof(env));

            _PythonEnvironment = env;
        }

        #endregion

        #region Public-Methods

        /// <summary>
        /// Initialize the virtual environment specified in the PythonEnvironment object.
        /// </summary>
        public void InitializeVirtualEnvironment()
        {
            string cmd = "";
            int result = 0;

            if (String.IsNullOrEmpty(_PythonEnvironment.VirtualEnvironmentPath))
            {
                Log(Severity.Debug, "no virtual environment path specified in python environment");
                return;
            }

            if (!Directory.Exists(_PythonEnvironment.VirtualEnvironmentPath))
            {
                Log(Severity.Info, "specified virtual environment path '" + _PythonEnvironment.VirtualEnvironmentPath + "' does not exist, creating");

                Shelli shell = new Shelli();

                string pipBaseCommand = BuildPipBaseCommand();
                string pipReqCommand = BuildPipRequirementsCommand(_PythonEnvironment.RequirementsFile);
                cmd = "";

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    cmd =
                        PreparedPath(_PythonEnvironment.PythonExecutable) + " -m venv " + PreparedPath(_PythonEnvironment.VirtualEnvironmentPath) + " "
                        + "&& " + PreparedPath(_PythonEnvironment.VirtualEnvironmentPath + "/scripts/activate") + " "
                        + "&& " + pipBaseCommand + " "
                        + (!String.IsNullOrEmpty(pipReqCommand)
                            ? "&& " + pipReqCommand + " "
                            : "")
                        + "&& " + PreparedPath(_PythonEnvironment.VirtualEnvironmentPath + "/scripts/deactivate");
                }
                else if (
                    RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                    || RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD)
                    || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    cmd =
                        PreparedPath(_PythonEnvironment.PythonExecutable) + " -m venv " + PreparedPath(_PythonEnvironment.VirtualEnvironmentPath) + " "
                        + "; source " + PreparedPath(_PythonEnvironment.VirtualEnvironmentPath + "/bin/activate") + " "
                        + "; " + pipBaseCommand + " "
                        + (!String.IsNullOrEmpty(pipReqCommand)
                            ? "; " + pipReqCommand + " "
                            : "")
                        + "; deactivate";
                }
                else
                {
                    throw new Exception("Unable to determine operating system.");
                }

                result = shell.Go(cmd);
                _Logging.LogShellCommand(cmd, result);

                if (result != 0)
                    Log(Severity.Warn, "unable to setup virtual environment at " + _PythonEnvironment.VirtualEnvironmentPath);
                else
                    Log(Severity.Info, "created virtual environment at " + _PythonEnvironment.VirtualEnvironmentPath);
            }
            else
            {
                Log(Severity.Debug, "virtual environment exists at " + _PythonEnvironment.VirtualEnvironmentPath);
            }
        }

        /// <summary>
        /// Run a Python script.
        /// </summary>
        /// <param name="scriptFile">Script file.</param>
        /// <param name="inputData">Input data.</param>
        /// <param name="contentType">Content type.</param>
        /// <returns>PythonResult.</returns>
        public PythonResult RunScript(string scriptFile, string inputData, string contentType = "application/json")
        {
            if (String.IsNullOrEmpty(scriptFile)) throw new ArgumentNullException(nameof(scriptFile));
            if (String.IsNullOrEmpty(contentType)) throw new ArgumentNullException(nameof(contentType));
            scriptFile = scriptFile.Replace("\\", "/");
            
            if (inputData == null) inputData = "";

            #region Setup-Virtual-Environment

            if (!String.IsNullOrEmpty(_PythonEnvironment.VirtualEnvironmentPath))
            {
                InitializeVirtualEnvironment();
            }

            #endregion

            #region Process

            switch (_PythonEnvironment.ExecutionMode)
            {
                case ExecutionModeEnum.Filesystem:
                    return RunScriptFile(scriptFile, inputData, contentType);
                default:
                    throw new ArgumentException("Unknown execution mode '" + _PythonEnvironment.ExecutionMode.ToString() + "'.");
            }

            #endregion
        }

        #endregion

        #region Private-Methods

        private void Log(Severity sev, string msg)
        {
            if (String.IsNullOrEmpty(msg)) return;
            _Logging.Log(sev, msg);
        }

        private int ExecuteShellCommand(string cmd)
        {
            Shelli shelli = new Shelli();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                shelli.WindowsShell = _PythonEnvironment.ShellCommandWindows;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                || RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD)
                || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                shelli.LinuxShell = _PythonEnvironment.ShellCommandLinux;
            }
            else
            {
                throw new Exception("Unable to determine operating system.");
            }

            _Logging.LogShellCommand("executing command: " + cmd);

            int result = shelli.Go(cmd);

            _Logging.LogShellCommand("command result: " + cmd, result);

            return result;
        }

        private string PreparedPath(string path)
        {
            if (String.IsNullOrEmpty(path)) return path;

            path = Path.GetFullPath(path);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (path.Contains(" ")) path = "\"" + path + "\"";
            }
            else
            {

            }

            return path;
        }

        private string BuildPipBaseCommand()
        {
            return
                "pip install "
                + (_PythonEnvironment.QuietRequirementsInstallation ? "-q " : "")
                + "uvicorn fastapi";
        }

        private string BuildPipRequirementsCommand(string requirementsFile)
        {
            return
                "pip install "
                + (_PythonEnvironment.QuietRequirementsInstallation ? "-q " : "")
                + "-r " + Path.GetFullPath(_PythonEnvironment.RequirementsFile) + " ";
        }

        private PythonResult RunScriptFile(string scriptFile, string inputData, string contentType)
        { 
            #region Initialize-Shell

            Shelli shelli = new Shelli();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                shelli.WindowsShell = _PythonEnvironment.ShellCommandWindows;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                shelli.LinuxShell = _PythonEnvironment.ShellCommandLinux;
            }
            else if (
                RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD)
                || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                shelli.LinuxShell = _PythonEnvironment.ShellCommandMac;
            }
            else
            {
                throw new Exception("Unable to determine operating system.");
            }

            StringBuilder sbOutput = new StringBuilder();
            StringBuilder sbError = new StringBuilder();

            shelli.OutputDataReceived += (string msg) => sbOutput.Append(msg + Environment.NewLine);
            shelli.ErrorDataReceived += (string msg) => sbError.Append(msg + Environment.NewLine);

            #endregion

            #region Build-Full-Script

            string guid = Guid.NewGuid().ToString();
            _Logging.Log(Severity.Debug, "executing script using GUID " + guid);

            string userScriptFilename = PreparedPath(Path.Combine(_PythonEnvironment.ScriptPath, scriptFile));
            string[] userScriptLines = File.ReadAllLines(userScriptFilename);
            if (userScriptLines == null || userScriptLines.Length < 1)
            {
                _Logging.Log(Severity.Warn, "no contents found in script file " + userScriptFilename);
                throw new InvalidOperationException("No contents found in script file '" + userScriptFilename + "'.");
            }

            string masterScriptFilename = PreparedPath(Path.Combine(_PythonEnvironment.ScriptPath, guid + ".py"));
            string masterScriptResultFilename = PreparedPath(Path.Combine(_PythonEnvironment.ScriptPath, guid + ".resp"));

            string masterScript =
                "import json" + Environment.NewLine
                + Environment.NewLine
                + "# Begin user code" + Environment.NewLine
                + Environment.NewLine;

            foreach (string line in userScriptLines)
                masterScript += line + Environment.NewLine;

            masterScript +=
                Environment.NewLine
                + "# End user code" + Environment.NewLine
                + Environment.NewLine
                + "_req_obj   = json.loads('" + inputData.Replace("'", "\\'") + "')" + Environment.NewLine
                + "_resp_obj  = process(_req_obj)" + Environment.NewLine
                + "_out_file  = open('" + masterScriptResultFilename.Replace("\\", "\\\\") + "', 'w')" + Environment.NewLine
                + "_out_file.write(json.dumps(_resp_obj))" + Environment.NewLine;

            if (_PythonEnvironment.DisplayFullScript)
                _Logging.Log(Severity.Debug, "executing master script:" + Environment.NewLine + masterScript);

            File.WriteAllBytes(masterScriptFilename, Encoding.UTF8.GetBytes(masterScript));
            _Logging.Log(Severity.Debug, "wrote master script to " + masterScriptFilename);

            #endregion

            #region Execute

            List<string> cmdEntriesList = new List<string>();
            
            if (!String.IsNullOrEmpty(_PythonEnvironment.VirtualEnvironmentPath))
            {
                cmdEntriesList.Add("cd " + PreparedPath(_PythonEnvironment.VirtualEnvironmentPath));
                cmdEntriesList.Add(PreparedPath(_PythonEnvironment.PythonExecutable) + " -m venv " + PreparedPath(_PythonEnvironment.VirtualEnvironmentPath));
                cmdEntriesList.Add(PreparedPath(_PythonEnvironment.VirtualEnvironmentPath + "/scripts/activate"));
            }

            if (_PythonEnvironment.DisplayPipPackages)
                cmdEntriesList.Add("pip list");

            cmdEntriesList.Add("cd " + PreparedPath(_PythonEnvironment.ScriptPath));
            cmdEntriesList.Add(PreparedPath(_PythonEnvironment.PythonExecutable) + " " + masterScriptFilename);

            if (!String.IsNullOrEmpty(_PythonEnvironment.VirtualEnvironmentPath))
                cmdEntriesList.Add("deactivate");

            string[] cmdEntriesArray = cmdEntriesList.ToArray();
            string cmd = "";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                for (int i = 0; i < cmdEntriesArray.Length; i++)
                {
                    if (i == 0) cmd += cmdEntriesArray[i] + " ";
                    else cmd += "&& " + cmdEntriesArray[i] + " ";
                }
            }
            else if (
                RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                || RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD)
                || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                for (int i = 0; i < cmdEntriesArray.Length; i++)
                {
                    if (i == 0) cmd += cmdEntriesArray[i] + " ";
                    else cmd += "; " + cmdEntriesArray[i] + " ";
                }
            }
            else
            {
                throw new Exception("Unable to determine operating system.");
            }

            PythonResult result = new PythonResult();

            try
            {
                _Logging.LogShellCommand(cmd);

                result.Result = shelli.Go(cmd);
                
                if (result.Result == 0) result.Success = true;
                else result.Success = false;

                result.ConsoleOutput = sbOutput.ToString();
                result.ErrorOutput = sbError.ToString();

                _Logging.Log(Severity.Debug, "cleaning up script file " + masterScriptFilename);
                File.Delete(masterScriptFilename);

                if (File.Exists(masterScriptResultFilename))
                {
                    _Logging.Log(Severity.Debug, "reading result from " + masterScriptResultFilename);
                    result.Data = File.ReadAllText(masterScriptResultFilename);

                    _Logging.Log(Severity.Debug, "cleaning up result file " + masterScriptResultFilename);
                    File.Delete(masterScriptResultFilename);
                }

                return result;
            }
            catch (Exception e)
            {
                _Logging.Log(Severity.Warn, "exception encountered in script execution: " + Environment.NewLine + e.ToString());

                result.Success = false;
                result.Exception = e;
                result.ConsoleOutput = sbOutput.ToString();
                result.ErrorOutput = sbError.ToString();

                return result;
            }
            finally
            {
            }

            #endregion
        }

        #endregion
    }
}
