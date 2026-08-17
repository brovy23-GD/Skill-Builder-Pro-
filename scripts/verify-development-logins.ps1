$ErrorActionPreference = 'Stop'

$project = Join-Path $PSScriptRoot '..\SkillBuilderPro.API\SkillBuilderPro.API.csproj'
$secretLines = & dotnet user-secrets list --project $project
$secrets = @{}
foreach ($line in $secretLines) {
    if ($line -match '^([^=]+?)\s*=\s*(.*)$') {
        $secrets[$matches[1].Trim()] = $matches[2]
    }
}

$checks = @(
    @{ Section = 'DevelopmentAthlete'; Role = 'Athlete' },
    @{ Section = 'DevelopmentParent'; Role = 'Parent' },
    @{ Section = 'DevelopmentCoach'; Role = 'Coach' },
    @{ Section = 'DevelopmentAdmin'; Role = 'Administrator' }
)

foreach ($check in $checks) {
    $email = $secrets["$($check.Section):Email"]
    $password = $secrets["$($check.Section):Password"]
    if ([string]::IsNullOrWhiteSpace($email) -or [string]::IsNullOrWhiteSpace($password)) {
        Write-Output "$($check.Role): CONFIGURATION MISSING"
        continue
    }

    try {
        $body = @{ email = $email; password = $password } | ConvertTo-Json
        $response = Invoke-RestMethod -Method Post -Uri 'http://127.0.0.1:5000/api/auth/login' -ContentType 'application/json' -Body $body
        $roleOk = $response.user.roles -contains $check.Role
        $tokenOk = -not [string]::IsNullOrWhiteSpace($response.accessToken)
        if ($roleOk -and $tokenOk) { Write-Output "$($check.Role): PASS" }
        else { Write-Output "$($check.Role): FAIL (sanitized role/token validation)" }
    }
    catch {
        $status = if ($_.Exception.Response) { [int]$_.Exception.Response.StatusCode } else { 'NO_RESPONSE' }
        Write-Output "$($check.Role): FAIL (HTTP $status)"
    }
}

try {
    $health = Invoke-WebRequest -UseBasicParsing -Uri 'http://127.0.0.1:5000/health'
    Write-Output "API HEALTH: HTTP $($health.StatusCode)"
}
catch {
    Write-Output 'API HEALTH: FAIL'
}
