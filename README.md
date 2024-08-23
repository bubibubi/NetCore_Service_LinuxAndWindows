# .Net Core Service sample for Windows and Linux
## Introduction
Since .Net core, Windows services interface has totally been rewritten.
Also the automatic installation cannot be implemented using standard .Net Core libraries.
This is an implementation that can run the executable as
* Console application
* Windows service (if we're on a Windows machine)
* Linux deamon (if we're on a Linux machine)

On Windows, the executable can be installed and removed from the services using the service
The executable uses SC.EXE

## Implementation
* Start from a standard WorkerService
* Adding/Changing code for run as a Windows service
    * Adding Microsoft.Extensions.Hosting.WindowsServices Nuget package
    * Change builder to execute UseWindowsService() extension method (in case of WindowsService)
* Adding code to install and remove as Windows service

## Linux systemd
For systemd look this article
https://devblogs.microsoft.com/dotnet/net-core-and-systemd/
