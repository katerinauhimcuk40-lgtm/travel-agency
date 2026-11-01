# Локальний звіт покриття: .\scripts\coverage.ps1  (з кореня репозиторію)
param([int]$Threshold = 0)

$ErrorActionPreference = "Stop"
if (Test-Path coverage) { Remove-Item coverage -Recurse -Force }

dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura `
    "/p:CoverletOutput=$PWD/coverage/coverage.cobertura.xml" `
    /p:Threshold=$Threshold /p:ThresholdType=line /p:ThresholdStat=total
$testExit = $LASTEXITCODE

if (-not (Get-Command reportgenerator -ErrorAction SilentlyContinue)) {
    dotnet tool install -g dotnet-reportgenerator-globaltool
}
reportgenerator -reports:coverage/coverage.cobertura.xml -targetdir:coverage/report -reporttypes:"Html;TextSummary"
Get-Content coverage/report/Summary.txt | Select-Object -First 12
Write-Host "HTML-звіт: coverage/report/index.html"
exit $testExit
