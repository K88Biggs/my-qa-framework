using NUnit.Framework;
using PlaywrightTestFramework.Core.Tests;
using PlaywrightTestFramework.Pages;
using PlaywrightTestFramework.Models;

namespace PlaywrightTestFramework.Tests;

[TestFixture]
public class LoginTests : BaseTest
{
    private LoginPage _loginPage = null!;

    [SetUp]
    public async Task LoginTestSetUp()
    {
        _loginPage = new LoginPage(Page!, Config);
        await _loginPage.NavigateToAsync("https://example.com/login");
    }

    [Test]
    [Category("Positive")]
    [Description("Verify successful login with valid credentials")]
    public async Task Login_WithValidCredentials_ShouldSucceed()
    {
        // Arrange
        var testUser = new User
        {
            Username = "validuser@example.com",
            Password = "ValidPassword123!"
        };

        // Act
        var homePage = await _loginPage.LoginAsync(testUser.Username, testUser.Password);
        
        // Assert
        var welcomeMessage = await homePage.GetWelcomeMessageAsync();
        Assert.That(welcomeMessage, Does.Contain("Welcome"), "Welcome message should be displayed after successful login");
        
        var currentUrl = await homePage.GetCurrentUrlAsync();
        Assert.That(currentUrl, Does.Contain("/home"), "Should navigate to home page after login");
    }

    [Test]
    [Category("Positive")]
    [Description("Verify login form UI components are displayed correctly")]
    public async Task LoginForm_UIComponents_ShouldBeDisplayedCorrectly()
    {
        // Act & Assert
        var isFormValid = await _loginPage.ValidateLoginFormComponentsAsync();
        Assert.That(isFormValid, Is.True, "All login form components should be visible and accessible");

        var isLoginButtonEnabled = await _loginPage.IsLoginButtonEnabledAsync();
        Assert.That(isLoginButtonEnabled, Is.True, "Login button should be enabled by default");
    }

    [Test]
    [Category("Negative")]
    [Description("Verify login fails with invalid credentials")]
    public async Task Login_WithInvalidCredentials_ShouldFail()
    {
        // Arrange
        var invalidUser = new User
        {
            Username = "invalid@example.com",
            Password = "WrongPassword"
        };

        // Act
        await _loginPage.LoginAsync(invalidUser.Username, invalidUser.Password);
        
        // Assert
        var isErrorDisplayed = await _loginPage.IsErrorMessageDisplayedAsync();
        Assert.That(isErrorDisplayed, Is.True, "Error message should be displayed for invalid credentials");
        
        var errorMessage = await _loginPage.GetErrorMessageAsync();
        Assert.That(errorMessage, Does.Contain("Invalid").Or.Contain("incorrect"), 
            "Error message should indicate invalid credentials");
    }

    [Test]
    [Category("Negative")]
    [Description("Verify username field validation")]
    public async Task Login_WithEmptyUsername_ShouldShowValidationError()
    {
        // Act & Assert
        var hasValidationError = await _loginPage.ValidateUsernameRequiredAsync();
        Assert.That(hasValidationError, Is.True, "Validation error should be shown when username is empty");
    }

    [Test]
    [Category("Negative")]
    [Description("Verify password field validation")]
    public async Task Login_WithEmptyPassword_ShouldShowValidationError()
    {
        // Act & Assert
        var hasValidationError = await _loginPage.ValidatePasswordRequiredAsync();
        Assert.That(hasValidationError, Is.True, "Validation error should be shown when password is empty");
    }

    [Test]
    [Category("Negative")]
    [Description("Verify SQL injection protection")]
    public async Task Login_WithSQLInjection_ShouldBeBlocked()
    {
        // Arrange
        var maliciousInput = "admin'; DROP TABLE users; --";
        
        // Act
        await _loginPage.LoginAsync(maliciousInput, "password");
        
        // Assert
        var isErrorDisplayed = await _loginPage.IsErrorMessageDisplayedAsync();
        Assert.That(isErrorDisplayed, Is.True, "SQL injection attempts should be blocked and show error");
        
        var currentUrl = await _loginPage.GetCurrentUrlAsync();
        Assert.That(currentUrl, Does.Contain("/login"), "Should remain on login page after injection attempt");
    }
}
