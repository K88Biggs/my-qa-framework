using Microsoft.Playwright;
using NUnit.Framework;
using PlaywrightTestFramework.Core.Configuration;
using PlaywrightTestFramework.Core.Database;
using PlaywrightTestFramework.Core.API;
using Serilog;

namespace PlaywrightTestFramework.Core.Tests;

[TestFixture]
public class BaseTest
{
    protected IPlaywright? Playwright;
    protected IBrowser? Browser;
    protected IPage? Page;
    protected TestConfiguration Config = null!;
    protected MongoDbHelper DbHelper = null!;
    protected ApiHelper ApiHelper = null!;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        // Configure logging
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("Logs/test-log-.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        Config = new TestConfiguration();
        DbHelper = new MongoDbHelper(Config);
        ApiHelper = new ApiHelper(Config);
        
        // Install playwright browsers if needed
        Microsoft.Playwright.Program.Main(new[] { "install" });
        
        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
    }

    [SetUp]
    public async Task SetUp()
    {
        // Launch browser for each test
        Browser = await Playwright!.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = Config.BrowserHeadless,
            SlowMo = Config.BrowserSlowMo
        });

        var context = await Browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize
            {
                Width = Config.ViewportWidth,
                Height = Config.ViewportHeight
            },
            RecordVideoDir = Config.RecordVideos ? Path.Combine(Config.ReportPath, "Videos") : null
        });

        Page = await context.NewPageAsync();
        Page.SetDefaultTimeout(Config.BrowserTimeout);
    }

    [TearDown]
    public async Task TearDown()
    {
        if (Config.TakeScreenshots && TestContext.CurrentContext.Result.Outcome.Status != NUnit.Framework.Interfaces.TestStatus.Passed)
        {
            var screenshotPath = Path.Combine(Config.ReportPath, "Screenshots", $"{TestContext.CurrentContext.Test.Name}_{DateTime.Now:yyyyMMdd_HHmmss}.png");
            Directory.CreateDirectory(Path.GetDirectoryName(screenshotPath)!);
            await Page!.ScreenshotAsync(new() { Path = screenshotPath });
        }

        await Browser?.CloseAsync()!;
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        Playwright?.Dispose();
        Log.CloseAndFlush();
    }
}
