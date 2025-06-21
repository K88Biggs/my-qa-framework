using Microsoft.Extensions.Configuration;

namespace PlaywrightTestFramework.Core.Configuration;

public class TestConfiguration
{
    private readonly IConfiguration _configuration;

    public TestConfiguration()
    {
        _configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
    }

    public string MongoConnectionString => _configuration["MongoDB:ConnectionString"]!;
    public string MongoDatabaseName => _configuration["MongoDB:DatabaseName"]!;
    public string ApiBaseUrl => _configuration["API:BaseUrl"]!;
    public int ApiTimeout => int.Parse(_configuration["API:Timeout"]!);
    public bool BrowserHeadless => bool.Parse(_configuration["Browser:Headless"]!);
    public int BrowserSlowMo => int.Parse(_configuration["Browser:SlowMo"]!);
    public int BrowserTimeout => int.Parse(_configuration["Browser:Timeout"]!);
    public int ViewportWidth => int.Parse(_configuration["Browser:ViewportWidth"]!);
    public int ViewportHeight => int.Parse(_configuration["Browser:ViewportHeight"]!);
    public string ReportPath => _configuration["Reporting:ReportPath"]!;
    public bool TakeScreenshots => bool.Parse(_configuration["Reporting:Screenshots"]!);
    public bool RecordVideos => bool.Parse(_configuration["Reporting:Videos"]!);
}
