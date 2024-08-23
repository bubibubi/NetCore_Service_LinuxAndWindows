using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.Hosting.WindowsServices;
using WorkerService1;
using OperatingSystem = System.OperatingSystem;

string serviceName = "MyService";

string verb = args.Length == 0 ? "-run" : args[0].ToLower();

switch (verb)
{
    case "-run":
        // Just run as daemon or over console
        break;
    case "-install":
        Console.WriteLine($"Installing '{serviceName}' as a service");
        return RunScCommand("create", serviceName + " binPath= \"" + Assembly.GetExecutingAssembly().Location + "\"");
    case "-remove":
        Console.WriteLine($"Removing '{serviceName}' from services");
        return RunScCommand("delete", serviceName);
    default:
        WriteHelpToConsole();
        return 1;
}

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
    {
        services.AddHostedService<Worker>();
    });

if (WindowsServiceHelpers.IsWindowsService())
{
    builder.UseWindowsService();
}

var host = builder.Build();

host.Run();

return 0;

static void WriteHelpToConsole()
{
    string executableName = Assembly.GetExecutingAssembly().GetName().Name!;
    if (OperatingSystem.IsWindows())
    {
        Console.WriteLine($"Usage: {executableName}.exe [-install|-remove|-run]");
        Console.WriteLine();
        Console.WriteLine("-run\truns the application (default)");
        Console.WriteLine("-install\tInstalls the executable as a Windows service");
        Console.WriteLine("-remove\tremoves the executable from the Windows services");
    }
    else
    {
        Console.WriteLine($"Usage: {executableName}.exe");
        Console.WriteLine();
        Console.WriteLine("You can run the application as a deamon");
    }
}


static int RunScCommand(string command, string arguments)
{
    ProcessStartInfo processStartInfo = new ProcessStartInfo
    {
        FileName = "sc.exe",
        Arguments = $"{command} {arguments}",
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false,
        CreateNoWindow = true
    };

    using Process process = new Process();
    
    process.StartInfo = processStartInfo;

    process.Start();
    string output = process.StandardOutput.ReadToEnd();
    string error = process.StandardError.ReadToEnd();
    process.WaitForExit();

    Console.WriteLine(output);
    if (!string.IsNullOrEmpty(error))
    {
        Console.WriteLine("Error: " + error);
    }

    return process.ExitCode;
}