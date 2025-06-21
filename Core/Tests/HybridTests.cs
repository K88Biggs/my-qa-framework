using NUnit.Framework;
using PlaywrightTestFramework.Core.Tests;
using PlaywrightTestFramework.Pages;
using PlaywrightTestFramework.Models;
using System.Net;

namespace PlaywrightTestFramework.Tests;

[TestFixture]
public class HybridTests : BaseTest
{
    private LoginPage _loginPage = null!;
    private HomePage _homePage = null!;

    [SetUp]
    public async Task HybridTestSetUp()
    {
        _loginPage = new LoginPage(Page!, Config);
        await _loginPage.NavigateToAsync("https://example.com/login");
    }

    [Test]
    [Category("Hybrid")]
    [Description("Verify end-to-end user registration flow with API and DB validation")]
    public async Task UserRegistration_EndToEnd_ShouldWork()
    {
        // Arrange - Create test user data
        var newUser = new User
        {
            Id = Guid.NewGuid().ToString(),
            Username = $"testuser_{DateTime.Now:yyyyMMdd_HHmmss}",
            Email = $"test_{DateTime.Now:yyyyMMdd_HHmmss}@example.com",
            Password = "TestPassword123!",
            FirstName = "Test",
            LastName = "User"
        };

        // Step 1: API - Create user via API
        var apiResponse = await ApiHelper.PostAsync<User>("/users", newUser);
        Assert.That(apiResponse.StatusCode, Is.EqualTo(HttpStatusCode.Created), 
            "User creation API should return 201 Created");
        Assert.That(ApiHelper.ValidateResponseTime(apiResponse, 5000), Is.True, 
            "API response time should be under 5 seconds");

        // Step 2: Database - Verify user was stored in database
        await DbHelper.InsertDocumentAsync("users", newUser);
        var storedUser = await DbHelper.GetDocumentByIdAsync<User>("users", newUser.Id);
        Assert.That(storedUser, Is.Not.Null, "User should be stored in database");
        Assert.That(storedUser.Username, Is.EqualTo(newUser.Username), 
            "Stored username should match original");

        // Step 3: UI - Login with new user credentials
        _homePage = await _loginPage.LoginAsync(newUser.Username, newUser.Password);
        var welcomeMessage = await _homePage.GetWelcomeMessageAsync();
        Assert.That(welcomeMessage, Does.Contain(newUser.FirstName), 
            "Welcome message should contain user's first name");

        // Cleanup
        await DbHelper.DeleteDocumentAsync<User>("users", newUser.Id);
    }

    [Test]
    [Category("Hybrid")]
    [Description("Verify product search with API data validation")]
    public async Task ProductSearch_WithAPIValidation_ShouldWork()
    {
        // Arrange - Setup test data in database
        var testProducts = new List<Product>
        {
            new() { Id = "1", Name = "Gaming Laptop", Price = 1500.00m, Category = "Electronics", InStock = true },
            new() { Id = "2", Name = "Office Laptop", Price = 800.00m, Category = "Electronics", InStock = true },
            new() { Id = "3", Name = "Gaming Mouse", Price = 50.00m, Category = "Accessories", InStock = false }
        };

        await DbHelper.SeedTestDataAsync("products", testProducts);

        // Step 1: UI - Navigate to home and perform search
        _homePage = new HomePage(Page!, Config);
        await _homePage.NavigateToAsync("https://example.com/home");
        
        var searchResultsPage = await _homePage.SearchAsync("laptop");
        var hasResults = await searchResultsPage.HasResultsAsync();
        Assert.That(hasResults, Is.True, "UI search should return results");

        // Step 2: API - Validate same search via API
        var apiSearchParams = new Dictionary<string, string> { { "q", "laptop" } };
        var apiResponse = await ApiHelper.GetAsync<List<Product>>("/products/search", apiSearchParams);
        Assert.That(apiResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK), 
            "API search should return 200 OK");
        Assert.That(apiResponse.Data?.Count, Is.GreaterThan(0), 
            "API should return matching products");

        // Step 3: Database - Verify data consistency
        var dbProducts = await DbHelper.GetCollectionAsync<Product>("products");
        var laptopProducts = dbProducts.Where(p => p.Name.ToLower().Contains("laptop")).ToList();
        Assert.That(laptopProducts.Count, Is.EqualTo(apiResponse.Data?.Count), 
            "Database and API should return same number of products");

        // Cleanup
        await DbHelper.CleanupCollectionAsync<Product>("products");
    }

    [Test]
    [Category("Hybrid")]
    [Description("Verify user profile update across all layers")]
    public async Task UserProfileUpdate_AllLayers_ShouldSync()
    {
        // Arrange - Setup test user
        var testUser = new User
        {
            Id = Guid.NewGuid().ToString(),
            Username = "profiletest@example.com",
            Email = "profiletest@example.com",
            FirstName = "John",
            LastName = "Doe",
            Password = "TestPassword123!"
        };

        await DbHelper.InsertDocumentAsync("users", testUser);

        // Step 1: UI - Login and navigate to profile
        _homePage = await _loginPage.LoginAsync(testUser.Username, testUser.Password);
        // Assume navigation to profile page exists
        
        // Step 2: API - Update user profile
        var updatedUser = new User
        {
            Id = testUser.Id,
            Username = testUser.Username,
            Email = testUser.Email,
            FirstName = "Jane",
            LastName = "Smith",
            Password = testUser.Password
        };

        var updateResponse = await ApiHelper.PutAsync<User>($"/users/{testUser.Id}", updatedUser);
        Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK), 
            "Profile update API should return 200 OK");

        // Step 3: Database - Verify update persisted
        var dbUser = await DbHelper.GetDocumentByIdAsync<User>("users", testUser.Id);
        Assert.That(dbUser.FirstName, Is.EqualTo("Jane"), 
            "Database should reflect updated first name");
        Assert.That(dbUser.LastName, Is.EqualTo("Smith"), 
            "Database should reflect updated last name");

        // Step 4: UI - Verify changes reflected in UI
        await Page!.ReloadAsync();
        var updatedWelcomeMessage = await _homePage.GetWelcomeMessageAsync();
        Assert.That(updatedWelcomeMessage, Does.Contain("Jane"), 
            "UI should show updated user name");

        // Cleanup
        await DbHelper.DeleteDocumentAsync<User>("users", testUser.Id);
    }

    [Test]
    [Category("Hybrid")]
    [Description("Verify data integrity across UI actions and backend")]
    public async Task DataIntegrity_UIActions_ShouldMatchBackend()
    {
        // Arrange - Login first
        var testUser = new User
        {
            Username = "datatest@example.com",
            Password = "TestPassword123!"
        };

        _homePage = await _loginPage.LoginAsync(testUser.Username, testUser.Password);

        // Step 1: UI - Perform multiple actions (search, filter, etc.)
        var searchTerm = "electronics";
        var searchResultsPage = await _homePage.SearchAsync(searchTerm);
        var uiResultCount = await searchResultsPage.GetResultCountAsync();

        // Step 2: API - Verify same action via API
        var apiParams = new Dictionary<string, string> { { "category", "electronics" } };
        var apiResponse = await ApiHelper.GetAsync<List<Product>>("/products", apiParams);
        var apiResultCount = apiResponse.Data?.Count ?? 0;

        // Step 3: Database - Direct query to verify data
        var dbProducts = await DbHelper.GetCollectionAsync<Product>("products");
        var electronicsProducts = dbProducts.Where(p => p.Category.ToLower() == "electronics").ToList();

        // Assert - All layers should have consistent data
        Assert.That(uiResultCount, Is.EqualTo(apiResultCount), 
            "UI and API should return same result count");
        Assert.That(apiResultCount, Is.EqualTo(electronicsProducts.Count), 
            "API and Database should have consistent data");
    }
}
