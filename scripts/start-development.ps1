param(
    [ValidateSet('WinForms', 'MAUIWindows')]
    [string]$Client = 'WinForms'
)

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$apiProject = Join-Path $repositoryRoot 'SkillBuilderPro.API\SkillBuilderPro.API.csproj'
$clientProject = if ($Client -eq 'WinForms') {
    Join-Path $repositoryRoot 'SkillBuilderPro.WinForms\SkillBuilderPro.WinForms.csproj'
} else {
    Join-Path $repositoryRoot 'SkillBuilderPro.MAUI\SkillBuilderPro.MAUI.csproj'
}

$apiProcess = Start-Process dotnet -ArgumentList @('run', '--project', $apiProject) -WorkingDirectory $repositoryRoot -WindowStyle Hidden -PassThru

try {
    $ready = $false
    foreach ($attempt in 1..30) {
        try {
            $response = Invoke-WebRequest 'http://localhost:5000/health' -UseBasicParsing -TimeoutSec 1
            if ($response.StatusCode -eq 200) { $ready = $true; break }
        } catch {
            Start-Sleep -Milliseconds 500
        }
    }

    if (-not $ready) {
        throw 'Skill Builder Pro API did not become healthy at http://localhost:5000 within 15 seconds.'
    }

    $arguments = @('run', '--project', $clientProject)
    if ($Client -eq 'MAUIWindows') { $arguments += @('-f', 'net10.0-windows10.0.19041.0') }
    & dotnet @arguments
}
finally {
    if (-not $apiProcess.HasExited) { Stop-Process -Id $apiProcess.Id }
}
