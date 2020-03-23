<#

.PURPOSE
    Test PSEntrypoint.exe to make sure it works correctly.

#>

[CmdletBinding()]
param(
    [ValidateSet('Debug','Release')]
    [string] $Configuration = 'Debug'
)


# set working directory

Set-Location $PSScriptRoot


# set subject under test

$subjectExe = "..\Entrypoint\bin\$Configuration\Entrypoint.exe"


# show help

& $subjectExe --help


# run test

start cmd.exe -ArgumentList @(
    '/c'
    $subjectExe
    'ping -t 127.0.0.1', '|| exit 1'
) -Wait


