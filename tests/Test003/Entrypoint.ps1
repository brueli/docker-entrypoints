<#
.PURPOSE
    Test script - Request a Shutdown after 5 cycles.
#>

# startup

# run code
$i = 0
while(!$entrypoint.Shutdown)
{
    Write-Verbose -Verbose -Message "Running"
    Start-Sleep -Seconds 1

    if (++$i -ge 5)
    {
        # request a shutdown after 5 cycles
        $entrypoint.RequestShutdown()
    }
}

# shutdown
Write-Host "Shutting down gracefully"