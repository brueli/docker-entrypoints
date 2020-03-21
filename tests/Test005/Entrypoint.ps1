<#
.PURPOSE
    Test script - throw an exception after 5 cycles, use a trap to report the fatal error
#>

# define the trap
trap {
    $entrypoint.ReportFatalError([Exception]::new("trapped: $_"))
}

# startup

# run code
$i = 0
while(!$entrypoint.Shutdown)
{
    Write-Verbose -Verbose -Message "Running"
    Start-Sleep -Seconds 1

    if (++$i -ge 5)
    {
        throw [Exception]::new("container image runtime error")
    }
}

# shutdown
Write-Host "Shutting down gracefully"