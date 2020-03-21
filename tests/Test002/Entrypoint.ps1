
# startup

# run code
while(!$entrypoint.Shutdown)
{
    Write-Verbose -Verbose -Message "Running"
    Start-Sleep -Seconds 1
}

# shutdown
Write-Host "Shutting down gracefully"