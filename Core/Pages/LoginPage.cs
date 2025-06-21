using Microsoft.Playwright;
using PlaywrightTestFramework.Core.Pages;
using PlaywrightTestFramework.Core.Configuration;

namespace PlaywrightTestFramework.Pages;

public class LoginPage : BasePage
{
    // Page elements
    private readonly string _usernameInput = "#username";
    private readonly string _passwordInput = "#password";
    private readonly string _loginButton = "#loginBtn";
    private readonly string _errorMessage = ".error-message";
    private readonly string _forgotPasswordLink = "#forgotPassword";
    private readonly string _rememberMeCheckbox = "#rememberMe";

    public LoginPage(IPage page, TestConfiguration config) : base(page, config) { }

    // Page actions
    public async Task EnterUsernameAsync(string username)
    {
        await FillInputAsync(_usernameInput, username);
    }

    public async Task EnterPasswordAsync(string password)
    {
        await FillInputAsync(_passwordInput, password);
    }

    public async Task ClickLoginButtonAsync()
    {
        await ClickElementAsync(_loginButton);
    }

    public async Task CheckRememberMeAsync()
    {
        await Page.CheckAsync(_rememberMeCheckbox);
    }

    public async Task ClickForgotPasswordAsync()
    {
        await ClickElementAsync(_forgotPasswordLink);
    }

    public async Task<HomePage> LoginAsync(string username, string password)
    {
        await EnterUsernameAsync(username);
        await EnterPasswordAsync(password);
        await ClickLoginButtonAsync();
        await WaitForPageLoad();
        return new HomePage(Page, Config);
    }

    // Validations
    public async Task<bool> IsLoginButtonEnabledAsync()
    {
        return await IsElementEnabledAsync(_loginButton);
    }

    public async Task<bool> IsErrorMessageDisplayedAsync()
    {
        return await IsElementVisibleAsync(_errorMessage);
    }

    public async Task<string> GetErrorMessageAsync()
    {
        return await GetTextAsync(_errorMessage);
    }

    public async Task<bool> ValidateUsernameRequiredAsync()
    {
        await EnterPasswordAsync("password");
        await ClickLoginButtonAsync();
        return await IsErrorMessageDisplayedAsync();
    }

    public async Task<bool> ValidatePasswordRequiredAsync()
    {
        await EnterUsernameAsync("username");
        await ClickLoginButtonAsync();
        return await IsErrorMessageDisplayedAsync();
    }

    // UI Component Validations
    public async Task<bool> ValidateLoginFormComponentsAsync()
    {
        var componentsValid = true;
        
        // Check if all required elements are present
        componentsValid &= await IsElementVisibleAsync(_usernameInput);
        componentsValid &= await IsElementVisibleAsync(_passwordInput);
        componentsValid &= await IsElementVisibleAsync(_loginButton);
        componentsValid &= await IsElementVisibleAsync(_forgotPasswordLink);
        
        Logger.Information($"Login form components validation: {componentsValid}");
        return componentsValid;
    }
}
