<#
.PURPOSE
    Test script - report a fatal error after 5 cycles
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
        $entrypoint.ReportFatalError([Exception]::new("Container image runtime error"))
    }
}

# shutdown
Write-Host "Shutting down gracefully"