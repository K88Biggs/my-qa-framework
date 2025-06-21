param(
    [string]$Category = "All",
    [string]$Browser = "chromium",
    [bool]$Headless = $false
)

Write-Host "Starting Playwright Test Execution..." -ForegroundColor Green
Write-Host "Category: $Category" -ForegroundColor Yellow
Write-Host "Browser: $Browser" -ForegroundColor Yellow
Write-Host "Headless: $Headless" -ForegroundColor Yellow

# Install Playwright browsers
Write-Host "Installing Playwright browsers..." -ForegroundColor Blue
dotnet build
pwsh bin/Debug/net8.0/playwright.ps1 install

# Set environment variables
$env:BROWSER = $Browser
$env:HEADLESS = $Headless.ToString().ToLower()

# Run tests based on category
if ($Category -eq "All") {
    dotnet test --logger "trx;LogFileName=TestResults.trx" --settings NUnit.runsettings
}
elseif ($Category -eq "Positive") {
    dotnet test --filter "Category=Positive" --logger "trx;LogFileName=PositiveTests.trx"
}
elseif ($Category -eq "Negative") {
    dotnet test --filter "Category=Negative" --logger "trx;LogFileName=NegativeTests.trx"
}
elseif ($Category -eq "Hybrid") {
    dotnet test --filter "Category=Hybrid" --logger "trx;LogFileName=HybridTests.trx"
}
else {
    dotnet test --filter "Category=$Category" --logger "trx;LogFileName=${Category}Tests.trx"
}

Write-Host "Test execution completed!" -ForegroundColor Green
Write-Host "Check TestResults folder for detailed reports." -ForegroundColor Cyan
