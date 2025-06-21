#!/bin/bash

CATEGORY=${1:-"All"}
BROWSER=${2:-"chromium"}
HEADLESS=${3:-"false"}

echo "Starting Playwright Test Execution..."
echo "Category: $CATEGORY"
echo "Browser: $BROWSER"
echo "Headless: $HEADLESS"

# Install Playwright browsers
echo "Installing Playwright browsers..."
dotnet build
pwsh bin/Debug/net8.0/playwright.ps1 install

# Set environment variables
export BROWSER=$BROWSER
export HEADLESS=$HEADLESS

# Run tests based on category
case $CATEGORY in
    "All")
        dotnet test --logger "trx;LogFileName=TestResults.trx" --settings NUnit.runsettings
        ;;
    "Positive")
        dotnet test --filter "Category=Positive" --logger "trx;LogFileName=PositiveTests.trx"
        ;;
    "Negative")
        dotnet test --filter "Category=Negative" --logger "trx;LogFileName=NegativeTests.trx"
        ;;
    "Hybrid")
        dotnet test --filter "Category=Hybrid" --logger "trx;LogFileName=HybridTests.trx"
        ;;
    *)
        dotnet test --filter "Category=$CATEGORY" --logger "trx;LogFileName=${CATEGORY}Tests.trx"
        ;;
esac

echo "Test execution completed!"
echo "Check TestResults folder for detailed reports."
        
