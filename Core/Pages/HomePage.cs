using Microsoft.Playwright;
using PlaywrightTestFramework.Core.Pages;
using PlaywrightTestFramework.Core.Configuration;

namespace PlaywrightTestFramework.Pages;

public class HomePage : BasePage
{
    // Page elements
    private readonly string _welcomeMessage = ".welcome-message";
    private readonly string _searchInput = "#searchInput";
    private readonly string _searchButton = "#searchBtn";
    private readonly string _userProfile = "#userProfile";
    private readonly string _logoutButton = "#logout";
    private readonly string _navigationMenu = ".nav-menu";
    private readonly string _searchResults = ".search-results";
    private readonly string _noResultsMessage = ".no-results";

    public HomePage(IPage page, TestConfiguration config) : base(page, config) { }

    // Page actions
    public async Task<string> GetWelcomeMessageAsync()
    {
        return await GetTextAsync(_welcomeMessage);
    }

    public async Task<SearchResultsPage> SearchAsync(string searchTerm)
    {
        await PerformSearchAsync(_searchInput, searchTerm, _searchButton);
        return new SearchResultsPage(Page, Config);
    }

    public async Task LogoutAsync()
    {
        await ClickElementAsync(_logoutButton);
        await WaitForPageLoad();
    }

    // Search field validations
    public async Task<bool> ValidateSearchFieldAsync()
    {
        var isVisible = await IsElementVisibleAsync(_searchInput);
        var isEnabled = await IsElementEnabledAsync(_searchInput);
        var placeholder = await Page.GetAttributeAsync(_searchInput, "placeholder");
        
        Logger.Information($"Search field validation - Visible: {isVisible}, Enabled: {isEnabled}, Placeholder: {placeholder}");
        return isVisible && isEnabled && !string.IsNullOrEmpty(placeholder);
    }

    public async Task<bool> ValidateEmptySearchAsync()
    {
        await FillInputAsync(_searchInput, "");
        await ClickElementAsync(_searchButton);
        
        // Should either prevent search or show appropriate message
        var hasNoResults = await IsElementVisibleAsync(_noResultsMessage);
        var searchInputHasFocus = await Page.EvaluateAsync<bool>($"document.querySelector('{_searchInput}') === document.activeElement");
        
        return hasNoResults || searchInputHasFocus;
    }

    public async Task<bool> ValidateSearchResultsDisplayAsync(string searchTerm)
    {
        await SearchAsync(searchTerm);
        return await IsElementVisibleAsync(_searchResults);
    }

    // UI Component Validations
    public async Task<bool> ValidateHomePageComponentsAsync()
    {
        var componentsValid = true;
        
        componentsValid &= await IsElementVisibleAsync(_welcomeMessage);
        componentsValid &= await IsElementVisibleAsync(_searchInput);
        componentsValid &= await IsElementVisibleAsync(_searchButton);
        componentsValid &= await IsElementVisibleAsync(_navigationMenu);
        componentsValid &= await IsElementVisibleAsync(_userProfile);
        
        Logger.Information($"Home page components validation: {componentsValid}");
        return componentsValid;
    }
}
