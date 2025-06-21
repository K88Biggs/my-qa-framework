using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;

namespace PlaywrightTestFramework.Core.Utilities;

public class ReportManager
{
    private static ExtentReports? _extent;
    private static ExtentTest? _test;

    public static void InitializeReport(string reportPath)
    {
        var htmlReporter = new ExtentSparkReporter(Path.Combine(reportPath, $"TestReport_{DateTime.Now:yyyyMMdd_HHmmss}.html"));
        _extent = new ExtentReports();
        _extent.AttachReporter(htmlReporter);
        
        _extent.AddSystemInfo("Environment", "Test");
        _extent.AddSystemInfo("Browser", "Chromium");
        _extent.AddSystemInfo("OS", Environment.OSVersion.ToString());
    }

    public static ExtentTest CreateTest(string testName, string description = "")
    {
        _test = _extent?.CreateTest(testName, description);
        return _test!;
    }

    public static void LogInfo(string message)
    {
        _test?.Info(message);
    }

    public static void LogPass(string message)
    {
        _test?.Pass(message);
    }

    public static void LogFail(string message)
    {
        _test?.Fail(message);
    }

    public static void LogScreenshot(string screenshotPath)
    {
        _test?.AddScreenCaptureFromPath(screenshotPath);
    }

    public static void FlushReport()
    {
        _extent?.Flush();
    }
}
