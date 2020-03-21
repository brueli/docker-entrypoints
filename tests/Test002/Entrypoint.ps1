
# startup

# run code
while(!$container.Shutdown)
{
    Write-Verbose -Verbose -Message "Running"
    Start-Sleep -Seconds 1
}

# shutdown
Write-Host "Shutting down gracefully"