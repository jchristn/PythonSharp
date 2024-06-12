<img src="https://github.com/jchristn/PythonSharp/blob/main/Assets/logo.png?raw=true" data-canonical-src="https://github.com/jchristn/PythonSharp/blob/main/Assets/logo.png?raw=true" width="128" height="128" />

# PythonSharp

PythonSharp is a simple library for invoking Python scripts from C# and retrieving their return value.

Why not just use [pythonnet](https://github.com/pythonnet/pythonnet)?  Pythonnet is a fantastic, useful, and well-written library, managed and maintained by an excellent community.  However, it is challenged in being able to support multiple concurrent virtual environments.  Further, pythonnet provides near seamless integration with the .NET CLR, whereas PythonSharp is a wrapper library that invokes Python scripts using the shell.  To summarize, **you should use pythonnet in most cases, but consider PythonSharp when you need to support multiple virtual environments concurrently**.

[![NuGet Version](https://img.shields.io/nuget/v/PythonSharp.svg?style=flat)](https://www.nuget.org/packages/PythonSharp/) [![NuGet](https://img.shields.io/nuget/dt/PythonSharp.svg)](https://www.nuget.org/packages/PythonSharp) 

## Help, Feedback, Contribute

If you have any issues or feedback, please file an issue here in Github. We'd love to have you help by contributing code for new features, optimization to the existing codebase, ideas for future releases, or fixes!

## New in v1.0.x

- Initial release

## Example Project

Refer to the ```Test``` project for exercising the library.  Create one instance of the `PythonRunner` class for each separate invocation.

```csharp
using PythonSharp;

// Create the environment object.
PythonEnvironment env = new PythonEnvironment();

// Set the base executable and pip command.
env.PythonExecutable = "C:\\Program Files\\Python312\\python.exe";
env.PipCommand = "C:\\Program Files\\Python312\\Scripts\\pip.exe";

// Set up the virtual environment, or skip.  If it does not exist, PythonSharp will create it.
env.VirtualEnvironmentPath = "./myscripts/venv/";

// Define path to the directory containing your script.
env.ScriptPath = "./myscripts/";

// Specify the requirements.txt file, or skip.
env.RequirementsFile = "./myscripts/requirements.txt";

// Create the script runner.
PythonRunner runner = new PythonRunner(env);
PythonResult result = runner.RunScript("script.py", "{ \"hello\": \"world\" }");
// {
//   "GUID": "[GUID]",                            // Unique identifier for the execution
//   "Success": true,                             // Success or failure
//   "CommandResult": 0,                          // Result of the shell command to run the script
//   "ConsoleOutput": "[Console messages here]",  // Console output encountered during execution
//   "ErrorOutput": "[Error messages here]",      // Error output encountered during execution
//   "Data": "{ \"hello\": \"world\" }";          // Data returned from the script
//   "Exception": null                            // Exception, if any
// }
```

## How Does it Work?

PythonSharp uses the [Shelli](https://github.com/jchristn/Shelli) library to invoke shell commands.  Under the hood, shell commands are issued when necessary to create virtual environments, install requirements (inside or outside of a virtual environment), enter and exit virtual environments, and to invoke scripts.

User scripts should be implemented with a `def process(req):` method.  The input `req` is a dictionary, and the output of this method must also be a dictionary.

Internally, a `Guid` is assigned for each invocation of a script.  This serves as an identifier for temporary files that are created during execution:
- A wrapper script, which encapsulates the user script
- An output file, containing a JSON representation of the dictionary response from the user script

The wrapper script is lightweight, and only serves to deserialize the input data supplied to `runner.RunScript` into a dictionary, and, to serialize the output data from the user function into a JSON object.  The user script is imported into the wrapper script, and saved as a temporary file called `[Guid].py`.  This file is deleted after execution.

The result value is stored in a file called `[Guid].resp` on the filesystem, and PythonSharp will read this file to obtain the results and then delete it.

The wrapper script, with a simple `def process(req):` integrated within, looks as follows when calling `runner.RunScript("script.py", "{ \"hello\": \"world\" }");`:
```python
import json

# Begin user code

def process(req):
    print("Hello from user code!")
    return req

# End user code

_req_obj   = json.loads('{ "hello": "world" }')
_resp_obj  = process(_req_obj)
_out_file  = open('9f786ef-7690-4024-8ec0-e5d699246527.resp', 'w')
_out_file.write(json.dumps(_resp_obj))
```

## User Script Requirements

- User scripts must implement a method `def process(req):` as the entrypoint for their code
- The input variable `req` to this method is a dictionary (deserialized from the JSON string supplied when invoking `runner.RunScript`)
- The `def process(req):` method should return a dictionary

## Version History

Refer to CHANGELOG.md for version history.
