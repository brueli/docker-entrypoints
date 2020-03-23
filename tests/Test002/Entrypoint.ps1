<#
.PURPOSE
    Test script - loop 20 times unless a TERM signal arrives
#>

# startup
Write-Host "Running in $PSScriptRoot"

# run code
$i = 0
while(!$entrypoint.Shutdown)
{
    Write-Verbose -Verbose -Message "Running"
    Start-Sleep -Seconds 1

    if (++$i -ge 20)
    {
        break
    }
}

# shutdown
Write-Host "Shutting down gracefully"