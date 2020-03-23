<#

.PURPOSE
    Test Entrypoint.exe to make sure it works correctly.

#>

[CmdletBinding()]
param(
    [ValidateSet('Debug','Release')]
    [string] $Configuration = 'Debug'
)


Set-Location $PSScriptRoot


# set subject under test

$subjectExe = "..\PSEntrypoint\bin\$Configuration\PSEntrypoint.exe"


# show help

& $subjectExe --help


# test 001: print all types of messages

start cmd.exe -ArgumentList @(
    '/c'
    $subjectExe
    '--entrypointScript', '.\Test001\Entrypoint.ps1'
    '--entrypointTimeout', 2000
    ' || exit 1'
) -Wait


# test 002: run 20 cycles unless TERM signal is received.

start cmd.exe -ArgumentList @(
    '/c'
    $subjectExe
    '--entrypointScript', '.\Test002\Entrypoint.ps1'
    '--shutdownScript', '.\Test002\Shutdown.ps1'
    '--entrypointTimeout', 2000
    ' || exit 1'
) -Wait


# test 003: $entrypoint.RequestShutdown()

start cmd.exe -ArgumentList @(
    '/c'
    $subjectExe
    '--entrypointScript', '.\Test003\Entrypoint.ps1'
    '--entrypointTimeout', 2000
    ' || exit 1'
) -Wait


# test 004: $entrypoint.ReportFatalError()

start cmd.exe -ArgumentList @(
    '/c'
    $subjectExe
    '--entrypointScript', '.\Test004\Entrypoint.ps1'
    '--entrypointTimeout', 2000
    ' || exit 1'
) -Wait


# test 005: use trap{ ... } to report any errors to $entrypoint.ReportFatalError()

start cmd.exe -ArgumentList @(
    '/c'
    $subjectExe
    '--entrypointScript', '.\Test005\Entrypoint.ps1'
    '--entrypointTimeout', 2000
    ' || exit 1'
) -Wait
