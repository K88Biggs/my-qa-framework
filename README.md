# Playwright Test Framework (.NET)

A comprehensive test automation framework built with Playwright, .NET, and following Page Object Model (POM) patterns.

## Features

- **Page Object Model (POM)** implementation
- **MongoDB** integration for database testing
- **RESTful API** testing capabilities
- **UI validations** for web components and search fields
- **Positive and Negative** test cases
- **Hybrid testing** (UI + API + Database)
- **Comprehensive reporting** with screenshots and videos
- **Data-driven testing** support
- **Cross-browser testing** capabilities

## Project Structure

```
PlaywrightTestFramework/
├── Core/
│   ├── Configuration/
│   │   └── TestConfiguration.cs
│   ├── Database/
│   │   └── MongoDbHelper.cs
│   ├── API/
│   │   └── ApiHelper.cs
│   ├── Pages/
│   │   └── BasePage.cs
│   ├── Tests/
│   │   └── BaseTest.cs
│   └── Utilities/
│       └── ReportManager.cs
├── Pages/
│   ├── LoginPage.cs
│   ├── HomePage.cs
│   └── SearchResultsPage.cs
├── Tests/
│   ├── LoginTests.cs
│   ├── SearchTests.cs
│   └── HybridTests.cs
├── Models/
│   ├── User.cs
│   └── Product.cs
├── TestData/
│   ├── TestDataGenerator.cs
│   ├── users.json
│   └── products.json
└── Scripts/
    ├── run-tests.ps1
    └── run-tests.sh
```

## Getting Started

### Prerequisites

- .NET 8.0 SDK
- MongoDB (for database tests)
- Node.js (for Playwright browser installation)

### Installation

1. Clone the repository
2. Restore NuGet packages:
   ```bash
   dotnet restore
   ```
3. Install Playwright browsers:
   ```bash
   dotnet build
   pwsh bin/Debug/net8.0/playwright.ps1 install
   ```

### Configuration

Update `appsettings.json` with your environment settings:

```json
{
  "MongoDB": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "TestDatabase"
  },
  "API": {
    "BaseUrl": "https://your-api-url.com",
    "Timeout": 30000
  },
  "Browser": {
    "Headless": false,
    "SlowMo": 100,
    "Timeout": 30000
  }
}
```

## Running Tests

### Run All Tests
```bash
dotnet test
```

### Run by Category
```bash
# Positive tests only
dotnet test --filter "Category=Positive"

# Negative tests only
dotnet test --filter "Category=Negative"

# Hybrid tests only
dotnet test --filter "Category=Hybrid"
```

### Using PowerShell Script
```powershell
.\Scripts\run-tests.ps1 -Category "Positive" -Browser "chromium" -Headless $false
```

### Using Bash Script
```bash
./Scripts/run-tests.sh "Positive" "chromium" "false"
```

## Test Categories

### Positive Tests
- Valid user login
- Successful search operations
- UI component validation
- Happy path scenarios

### Negative Tests
- Invalid credentials
- Empty field validations
- SQL injection protection
- Error handling scenarios

### Hybrid Tests
- End-to-end workflows
- UI + API + Database validation
- Data consistency checks
- Cross-layer integration tests

## Page Object Model Structure

### BasePage
- Common page interactions
- Element validation methods
- Screenshot utilities
- Logging capabilities

### Specific Pages
- LoginPage: Login functionality and validations
- HomePage: Search and navigation features
- SearchResultsPage: Search results and pagination

## Database Integration

The framework includes MongoDB integration for:
- Test data setup and cleanup
- Data validation across layers
- Database state verification
- CRUD operations testing

## API Integration

RESTful API testing capabilities:
- GET, POST, PUT, DELETE operations
- Response validation
- Performance testing
- Data consistency checks

## Reporting

- ExtentReports integration
- Screenshots on failure
- Video recording
- Detailed test logs
- Performance metrics

## Best Practices

1. **Page Object Model**: All page interactions are encapsulated in page classes
2. **Data-Driven Testing**: Test data is externalized in JSON files
3. **Hybrid Testing**: UI, API, and Database layers are tested together
4. **Comprehensive Validation**: Both positive and negative scenarios are covered
5. **Clean Architecture**: Clear separation of concerns and responsibilities

## Contributing

1. Follow the existing code structure
2. Add tests for new features
3. Update documentation
4. Ensure all tests pass before submitting
