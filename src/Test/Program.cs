namespace Test
{
    using System;
    using GetSomeInput;
    using PythonSharp;

    /// <summary>
    /// Program.
    /// </summary>
    public class Program
    {
        #region Public-Members

        #endregion

        #region Private-Members

        private static bool _RunForever = true;
        private static string _PythonExecutable = @"C:\Program Files\Python312\python.exe";
        private static string _PipCommand = @"C:\Program Files\Python312\Scripts\pip.exe";
        private static bool _QuietRequirementsInstallation = false;
        private static bool _LogShellCommands = true;
        private static bool _LogConsoleOutput = true;
        private static bool _LogErrorOutput = true;

        #endregion

        #region Entrypoint

        /// <summary>
        /// Main.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            Welcome();

            while (_RunForever)
            {
                string userInput = Inputty.GetString("Command [?/help]:", null, false);

                switch (userInput)
                {
                    case "?":
                        Menu();
                        break;
                    case "q":
                        _RunForever = false;
                        break;
                    case "c":
                    case "cls":
                        Console.Clear();
                        break;
                    case "sample1":
                        RunSample1();
                        break;
                    case "sample2":
                        RunSample2();
                        break;
                }
            }
        }

        #endregion

        #region Public-Methods

        #endregion

        #region Private-Methods

        private static void Welcome()
        {
            Console.WriteLine("");
            Console.WriteLine("PythonSharp Test Console");
            Console.WriteLine("");
            Console.WriteLine("Please set the variables '_PythonExecutable' and '_PipCommand'");
            Console.WriteLine("with full path and filename according to your environment.");
            Console.WriteLine("");
            Console.WriteLine("Currently using:");
            Console.WriteLine("| Python : " + _PythonExecutable);
            Console.WriteLine("| Pip    : " + _PipCommand);
            Console.WriteLine("");
        }

        private static void Menu()
        {
            Console.WriteLine("");
            Console.WriteLine("Available commands:");
            Console.WriteLine("  ?              Help, this menu");
            Console.WriteLine("  q              Exit the application");
            Console.WriteLine("  cls            Clear the console");
            Console.WriteLine("  sample1        Execute the sample1 application");
            Console.WriteLine("  sample2        Execute the sample2 application");
            Console.WriteLine("");
        }

        private static void EnumerateResult(PythonResult result)
        {
            Console.WriteLine("");
            Console.WriteLine("Result");
            if (result == null)
            {
                Console.WriteLine("(null)");
            }
            else
            {
                Console.WriteLine("| GUID            : " + result.GUID.ToString());
                Console.WriteLine("| Success         : " + result.Success);
                Console.WriteLine("| Commmand result : " + result.Result);
                Console.WriteLine("| Console output  : " + result.ConsoleOutput);
                Console.WriteLine("| Error output    : " + result.ErrorOutput);
                Console.WriteLine("| Data            : " + result.Data);
                Console.WriteLine("| Exception       : " + (result.Exception != null ? Environment.NewLine + result.Exception.ToString() : "(null)"));
            }

            Console.WriteLine("");
            return;
        }

        private static void Log(Severity sev, string msg)
        {
            Console.WriteLine(msg);
        }

        private static void RunSample1()
        {
            // no virtual environment

            string data = Inputty.GetString("Data:", null, true);

            PythonEnvironment env = new PythonEnvironment();
            env.PythonExecutable = _PythonExecutable;
            env.PipCommand = _PipCommand;
            env.RequirementsFile = null;
            env.ScriptPath = "./sample1/";
            env.VirtualEnvironmentPath = null;
            env.DisplayPipPackages = false;
            env.DisplayFullScript = true;
            env.ExecutionMode = ExecutionModeEnum.Filesystem;
            env.QuietRequirementsInstallation = _QuietRequirementsInstallation;

            PythonRunner runner = new PythonRunner(env);
            runner.Logging.LogShellCommands = _LogShellCommands;
            runner.Logging.LogConsoleOutput = _LogConsoleOutput;
            runner.Logging.LogErrorOutput = _LogErrorOutput;
            // runner.Logging.Logger = Log;

            PythonResult result = runner.RunScript("script.py", data);
            EnumerateResult(result);
        }

        private static void RunSample2()
        {
            // virtual environment

            string data = Inputty.GetString("Data:", null, true);

            PythonEnvironment env = new PythonEnvironment();
            env.PythonExecutable = _PythonExecutable;
            env.PipCommand = _PipCommand;
            env.RequirementsFile = "./sample2/requirements.txt";
            env.ScriptPath = "./sample2/";
            env.VirtualEnvironmentPath = "./sample2/venv2";
            env.DisplayPipPackages = false;
            env.ExecutionMode = ExecutionModeEnum.Filesystem;
            env.QuietRequirementsInstallation = _QuietRequirementsInstallation;

            PythonRunner runner = new PythonRunner(env);
            runner.Logging.LogShellCommands = _LogShellCommands;
            runner.Logging.LogConsoleOutput = _LogConsoleOutput;
            runner.Logging.LogErrorOutput = _LogErrorOutput;
            // runner.Logging.Logger = Log;

            PythonResult result = runner.RunScript("script.py", data);
            EnumerateResult(result);
        }

        #endregion
    }
}
