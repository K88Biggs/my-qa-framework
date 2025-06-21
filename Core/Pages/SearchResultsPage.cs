using Microsoft.Playwright;
using PlaywrightTestFramework.Core.Pages;
using PlaywrightTestFramework.Core.Configuration;

namespace PlaywrightTestFramework.Pages;

public class SearchResultsPage : BasePage
{
    // Page elements
    private readonly string _searchResults = ".search-results";
    private readonly string _resultItems = ".result-item";
    private readonly string _resultCount = ".result-count";
    private readonly string _noResultsMessage = ".no-results";
    private readonly string _paginationContainer = ".pagination";
    private readonly string _sortDropdown = "#sortBy";
    private readonly string _filterOptions = ".filter-options";

    public SearchResultsPage(IPage page, TestConfiguration config) : base(page, config) { }

    // Page actions
    public async Task<int> GetResultCountAsync()
    {
        var countText = await GetTextAsync(_resultCount);
        // Extract number from text like "Showing 25 results"
        var numbers = System.Text.RegularExpressions.Regex.Matches(countText, @"\d+");
        return numbers.Count > 0 ? int.Parse(numbers[0].Value) : 0;
    }

    public async Task<List<string>> GetResultTitlesAsync()
    {
        var elements = await Page.QuerySelectorAllAsync($"{_resultItems} .title");
        var titles = new List<string>();
        
        foreach (var element in elements)
        {
            var title = await element.TextContentAsync();
            if (!string.IsNullOrEmpty(title))
                titles.Add(title);
        }
        
        return titles;
    }

    public async Task SortResultsAsync(string sortOption)
    {
        await Page.SelectOptionAsync(_sortDropdown, sortOption);
        await WaitForPageLoad();
    }

    // Validations
    public async Task<bool> HasResultsAsync()
    {
        return await IsElementVisibleAsync(_resultItems);
    }

    public async Task<bool> HasNoResultsMessageAsync()
    {
        return await IsElementVisibleAsync(_noResultsMessage);
    }

    public async Task<bool> ValidateSearchResultsStructureAsync()
    {
        if (!await HasResultsAsync())
            return false;

        var resultElements = await Page.QuerySelectorAllAsync(_resultItems);
        
        foreach (var result in resultElements)
        {
            var hasTitle = await result.QuerySelectorAsync(".title") != null;
            var hasDescription = await result.QuerySelectorAsync(".description") != null;
            
            if (!hasTitle || !hasDescription)
                return false;
        }
        
        return true;
    }

    public async Task<bool> ValidatePaginationAsync()
    {
        var hasPagination = await IsElementVisibleAsync(_paginationContainer);
        
        if (!hasPagination)
            return true; // Pagination might not be needed for few results
            
        var hasNextButton = await Page.QuerySelectorAsync($"{_paginationContainer} .next") != null;
        var hasPrevButton = await Page.QuerySelectorAsync($"{_paginationContainer} .prev") != null;
        
        return hasNextButton || hasPrevButton;
    }
}
