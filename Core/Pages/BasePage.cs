using Microsoft.Playwright;
using PlaywrightTestFramework.Core.Configuration;
using Serilog;

namespace PlaywrightTestFramework.Core.Pages;

public abstract class BasePage
{
    protected readonly IPage Page;
    protected readonly TestConfiguration Config;
    protected readonly ILogger Logger;

    protected BasePage(IPage page, TestConfiguration config)
    {
        Page = page;
        Config = config;
        Logger = Log.ForContext(GetType());
    }

    // Common page actions
    public async Task NavigateToAsync(string url)
    {
        Logger.Information($"Navigating to: {url}");
        await Page.GotoAsync(url);
        await WaitForPageLoad();
    }

    public async Task WaitForPageLoad()
    {
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task<string> GetPageTitleAsync()
    {
        return await Page.TitleAsync();
    }

    public async Task<string> GetCurrentUrlAsync()
    {
        return Page.Url;
    }

    // Element interaction methods
    protected async Task ClickElementAsync(string selector)
    {
        Logger.Information($"Clicking element: {selector}");
        await Page.ClickAsync(selector);
    }

    protected async Task FillInputAsync(string selector, string value)
    {
        Logger.Information($"Filling input {selector} with: {value}");
        await Page.FillAsync(selector, value);
    }

    protected async Task<string> GetTextAsync(string selector)
    {
        var text = await Page.TextContentAsync(selector);
        Logger.Information($"Retrieved text from {selector}: {text}");
        return text ?? string.Empty;
    }

    protected async Task<bool> IsElementVisibleAsync(string selector, int timeout = 5000)
    {
        try
        {
            await Page.WaitForSelectorAsync(selector, new() { Timeout = timeout });
            return await Page.IsVisibleAsync(selector);
        }
        catch (TimeoutException)
        {
            return false;
        }
    }

    protected async Task<bool> IsElementEnabledAsync(string selector)
    {
        return await Page.IsEnabledAsync(selector);
    }

    // Validation methods
    protected async Task<bool> ValidateElementTextAsync(string selector, string expectedText)
    {
        var actualText = await GetTextAsync(selector);
        var isValid = actualText.Contains(expectedText, StringComparison.OrdinalIgnoreCase);
        Logger.Information($"Text validation - Expected: {expectedText}, Actual: {actualText}, Valid: {isValid}");
        return isValid;
    }

    protected async Task<bool> ValidateInputValueAsync(string selector, string expectedValue)
    {
        var actualValue = await Page.InputValueAsync(selector);
        var isValid = actualValue == expectedValue;
        Logger.Information($"Input validation - Expected: {expectedValue}, Actual: {actualValue}, Valid: {isValid}");
        return isValid;
    }

    // Search functionality
    protected async Task PerformSearchAsync(string searchSelector, string searchTerm, string submitSelector)
    {
        Logger.Information($"Performing search for: {searchTerm}");
        await FillInputAsync(searchSelector, searchTerm);
        await ClickElementAsync(submitSelector);
        await WaitForPageLoad();
    }

    // Screenshot utility
    public async Task TakeScreenshotAsync(string filename)
    {
        var screenshotPath = Path.Combine(Config.ReportPath, "Screenshots", $"{filename}_{DateTime.Now:yyyyMMdd_HHmmss}.png");
        Directory.CreateDirectory(Path.GetDirectoryName(screenshotPath)!);
        await Page.ScreenshotAsync(new() { Path = screenshotPath });
        Logger.Information($"Screenshot saved: {screenshotPath}");
    }
}
