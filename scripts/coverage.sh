#!/usr/bin/env bash
set -uo pipefail
THRESHOLD="${1:-0}"
ROOT="$(pwd -W 2>/dev/null || pwd)"
rm -rf coverage

dotnet test \
  -p:CollectCoverage=true \
  -p:CoverletOutputFormat=cobertura \
  "-p:CoverletOutput=$ROOT/coverage/coverage.cobertura.xml" \
  "-p:ExcludeByFile=**/Migrations/*.cs" \
  -p:Threshold="$THRESHOLD" -p:ThresholdType=line -p:ThresholdStat=total
code=$?

if [ ! -f coverage/coverage.cobertura.xml ]; then
  echo "Файл покриття не створено — див. помилки вище." >&2
  exit "${code:-1}"
fi

command -v reportgenerator >/dev/null 2>&1 || dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator -reports:coverage/coverage.cobertura.xml -targetdir:coverage/report -reporttypes:"Html;TextSummary"
head -12 coverage/report/Summary.txt
echo "HTML-звіт: coverage/report/index.html"
exit "$code"
