using NUnit.Framework;
using PlaywrightTestFramework.Core.Tests;
using PlaywrightTestFramework.Pages;

namespace PlaywrightTestFramework.Tests;

[TestFixture]
public class SearchTests : BaseTest
{
    private HomePage _homePage = null!;
    private SearchResultsPage _searchResultsPage = null!;

    [SetUp]
    public async Task SearchTestSetUp()
    {
        _homePage = new HomePage(Page!, Config);
        await _homePage.NavigateToAsync("https://example.com/home");
    }

    [Test]
    [Category("Positive")]
    [Description("Verify search functionality with valid search term")]
    public async Task Search_WithValidTerm_ShouldReturnResults()
    {
        // Arrange
        var searchTerm = "laptop";
        
        // Act
        _searchResultsPage = await _homePage.SearchAsync(searchTerm);
        
        // Assert
        var hasResults = await _searchResultsPage.HasResultsAsync();
        Assert.That(hasResults, Is.True, "Search should return results for valid term");
        
        var resultCount = await _searchResultsPage.GetResultCountAsync();
        Assert.That(resultCount, Is.GreaterThan(0), "Result count should be greater than 0");
        
        var resultTitles = await _searchResultsPage.GetResultTitlesAsync();
        Assert.That(resultTitles.Any(title => title.ToLower().Contains(searchTerm.ToLower())), 
            Is.True, "At least one result should contain the search term");
    }

    [Test]
    [Category("Positive")]
    [Description("Verify search field UI validations")]
    public async Task SearchField_UIValidations_ShouldPass()
    {
        // Act & Assert
        var isSearchFieldValid = await _homePage.ValidateSearchFieldAsync();
        Assert.That(isSearchFieldValid, Is.True, "Search field should be visible, enabled, and have placeholder text");
        
        var componentsValid = await _homePage.ValidateHomePageComponentsAsync();
        Assert.That(componentsValid, Is.True, "All home page components should be visible");
    }

    [Test]
    [Category("Positive")]
    [Description("Verify search results structure and pagination")]
    public async Task SearchResults_Structure_ShouldBeValid()
    {
        // Arrange
        var searchTerm = "computer";
        
        // Act
        _searchResultsPage = await _homePage.SearchAsync(searchTerm);
        
        // Assert
        var structureValid = await _searchResultsPage.ValidateSearchResultsStructureAsync();
        Assert.That(structureValid, Is.True, "Search results should have proper structure with titles and descriptions");
        
        var paginationValid = await _searchResultsPage.ValidatePaginationAsync();
        Assert.That(paginationValid, Is.True, "Pagination should be properly implemented when needed");
    }

    [Test]
    [Category("Negative")]
    [Description("Verify search with empty term")]
    public async Task Search_WithEmptyTerm_ShouldShowValidation()
    {
        // Act & Assert
        var emptySearchValid = await _homePage.ValidateEmptySearchAsync();
        Assert.That(emptySearchValid, Is.True, "Empty search should be handled gracefully");
    }

    [Test]
    [Category("Negative")]
    [Description("Verify search with non-existent term")]
    public async Task Search_WithNonExistentTerm_ShouldShowNoResults()
    {
        // Arrange
        var nonExistentTerm = "xyzabc12345nonexistent";
        
        // Act
        _searchResultsPage = await _homePage.SearchAsync(nonExistentTerm);
        
        // Assert
        var hasNoResultsMessage = await _searchResultsPage.HasNoResultsMessageAsync();
        Assert.That(hasNoResultsMessage, Is.True, "Should display 'no results' message for non-existent terms");
        
        var resultCount = await _searchResultsPage.GetResultCountAsync();
        Assert.That(resultCount, Is.EqualTo(0), "Result count should be 0 for non-existent terms");
    }

    [Test]
    [Category("Negative")]
    [Description("Verify search with special characters")]
    public async Task Search_WithSpecialCharacters_ShouldBeHandled()
    {
        // Arrange
        var specialCharacters = "!@#$%^&*()";
        
        // Act
        _searchResultsPage = await _homePage.SearchAsync(specialCharacters);
        
        // Assert - Should either return no results or handle gracefully
        var hasResults = await _searchResultsPage.HasResultsAsync();
        var hasNoResultsMessage = await _searchResultsPage.HasNoResultsMessageAsync();
        
        Assert.That(hasResults || hasNoResultsMessage, Is.True, 
            "Search with special characters should be handled gracefully");
    }
}
