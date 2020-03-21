DOCKER ENTRYPOINTS
==================

Docker entrypoints for windows with TERM signal support.


Usage
-----

* Install `entrypoint.exe` or `psentrypoint.exe` into your .NET Framework container image.
* Add an ENTRYPOINT directive to your `dockerfile`.


entrypoint.exe
==============

Windows docker entrypoint with Ctrl+C support for cmd.exe based command execution.

### Usage Examples

Run entrypoint command
```dockerfile
ENTRYPOINT [ "entrypoint.exe", "cmd.exe", "/c", "(net start MYSERVICE && ping -t 127.0.0.1) || (echo stopping... && net stop MYSERVICE)"]
```

Run entrypoint command with separate stop command
```dockerfile
ENTRYPOINT [ "entrypoint.exe", "cmd.exe", "/c", "net start MYSERVICE && ping -t 127.0.0.1", "--stop", "net stop MYSERVICE"]
```



psentrypoint.exe
================
Windows docker entrypoint with Ctrl+C support for powershell.exe based command execution.

### Usage Example

Run entrypoint script and a shutdown script with custom stop timeout.
```
ENTRYPOINT [ "psentrypoint.exe", "--entrypointScript", "C:\bin\entrypoint\Development.ps1", "--shutdownScript", "C:\bin\entrypoint\Shutdown.ps1", "--stop-timeout", "2000" ]
```

Run an entrypoint script and use a shutdown command.
```
ENTRYPOINT [ "psentrypoint.exe", "--entrypointScript", "C:\bin\entrypoint\Development.ps1", "--shutdownCommand", "Stop-Service mysql" ]
```

### Full Example

**Entrypoint.ps1**
```powershell
# startup

# run code
while(!$entrypoint.Shutdown)
{
    Write-Verbose -Verbose -Message "Running"
    Start-Sleep -Seconds 1
}

# shutdown
Write-Host "Shutting down gracefully"
```

**Shutdown.ps1**
```powershell
Write-Host "Shutting down"

Start-Sleep -Seconds 1

Write-Host "Shutdown complete"
```

**Dockerfile**
```dockerfile
ENTRYPOINT [ "psentrypoint.exe", "--entrypointScript", "Entrypoint.ps1", "--shutdownScript", "Shutdown.ps1" ]
```

**Output**
![Sample Output](docs/img/test002_executionoutput.png)


Special Features
----------------

### Interact with psentrypoint.exe from your scripts

Entrypoint commands/scripts can use the special `$container` variable to interact
with psentrypoint.exe.

The `$container` variable is an `IEntrypoint` object, provided by psentrypoint.exe, which is added to the PSHost so you can access it's properties and methods from your scripts.

```c#
interface IEntrypointState
{
    bool Shutdown { get; }
    void RequestShutdown();
    void ReportFatalError(Exception problem);
}
```

### Usage examples

**Request container shutdown from within your entrypoint**

```powershell
# start container
& C:\bin\entrypoint\scripts\Register-Runner.ps1
Start-Service gitlab-runner
Start-Sleep -Seconds 2

# run loop
while (!$entrypoint.Shutdown)
{
    Start-Sleep -Milliseconds 500

    # check if service is running
    $serviceRunning = (Get-Service gitlab-runner).Status -eq 'Running'
    if (!$serviceRunning)
    {
        # send shutdown request to psentrypoint.exe
        $entrypoint.RequestShutdown()
    }
}

# stop container
Stop-Service gitlab-runner
& C:\bin\entrypoint\scripts\Unregister-Runner.ps1
```

**Use a trap to catch exceptions**

```powershell
trap { 
    $entrypoint.ReportFatalError([Exception]::new("$_"))
}

# start container
& C:\bin\entrypoint\scripts\Register-Runner.ps1
Start-Service gitlab-runner
Start-Sleep -Seconds 2

# run loop
while (!$entrypoint.Shutdown)
{
    Start-Sleep -Milliseconds 500

    # check if service is running
    $serviceRunning = (Get-Service gitlab-runner).Status -eq 'Running'
    if (!$serviceRunning)
    {
        # throw an exception to trigger the trap
        throw "Service not running"
    }
}

# stop container
Stop-Service gitlab-runner
& C:\bin\entrypoint\scripts\Unregister-Runner.ps1
```