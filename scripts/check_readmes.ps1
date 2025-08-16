<#
PowerShell script to parse EnumaElishStructure.html and verify each declared folder has a README.md
Usage:
  powershell -NoProfile -ExecutionPolicy Bypass -File ".\scripts\check_readmes.ps1" -HtmlPath ".\EnumaElishStructure.html" -Root ".\GamePlatform"
#>
param(
    [string]$HtmlPath = ".\EnumaElishStructure.html",
    [string]$Root = ".\GamePlatform"
)

if (-not (Test-Path $HtmlPath)) {
    Write-Error "HTML file '$HtmlPath' not found. Ejecuta el script desde la raíz del repo o pasa la ruta correcta en -HtmlPath."
    exit 2
}

$raw = Get-Content $HtmlPath -Raw -ErrorAction Stop

# Find each project card block
$cardPattern = '(?s)<div\s+class="project-card.*?>(.*?)</div>'
[System.Text.RegularExpressions.Regex]::Options = 'Singleline'
$cardMatches = [regex]::Matches($raw, $cardPattern, [System.Text.RegularExpressions.RegexOptions]::Singleline)

$expectedFolders = New-Object System.Collections.Generic.List[string]

foreach ($card in $cardMatches) {
    $block = $card.Groups[1].Value

    # Module name: capture text after the icon span inside the h2
    $h2Pattern = '<h2[^>]*>.*?</span>\s*([^<]+)</h2>'
    $h2m = [regex]::Match($block, $h2Pattern, [System.Text.RegularExpressions.RegexOptions]::Singleline)
    if (-not $h2m.Success) { continue }
    $module = $h2m.Groups[1].Value.Trim()

    # Find li entries that have a bold span with the subfolder name like 'Launcher.UI:'
    $liPattern = '<li[^>]*>.*?<span[^>]*>([^<:]+):</span>'
    $liMatches = [regex]::Matches($block, $liPattern, [System.Text.RegularExpressions.RegexOptions]::Singleline)

    if ($liMatches.Count -gt 0) {
        foreach ($li in $liMatches) {
            $sub = $li.Groups[1].Value.Trim()
            $folder = Join-Path $Root $module
            $subfolder = Join-Path $folder $sub
            $expectedFolders.Add($subfolder) | Out-Null
        }
    } else {
        $expectedFolders.Add((Join-Path $Root $module)) | Out-Null
    }
}

# Ensure unique and also include root GamePlatform
$expected = $expectedFolders | Select-Object -Unique
$expected = @((Resolve-Path $Root -ErrorAction SilentlyContinue).Path) + $expected
$expected = $expected | Where-Object { $_ -ne $null } | Select-Object -Unique

$missing = @()
Write-Host "Comprobando $($expected.Count) rutas esperadas...`n"
foreach ($folder in $expected) {
    $readme = Join-Path $folder 'README.md'
    if (Test-Path $readme) {
        Write-Host "OK     : $readme"
    } else {
        Write-Host "MISSING: $readme" -ForegroundColor Yellow
        $missing += $readme
    }
}

if ($missing.Count -gt 0) {
    Write-Host "`nFaltan $($missing.Count) README(s)." -ForegroundColor Red
    exit 1
} else {
    Write-Host "`nTodos los README.md están presentes." -ForegroundColor Green
    exit 0
}
