using PlaywrightTestFramework.Models;

namespace PlaywrightTestFramework.TestData;

public static class TestDataGenerator
{
    public static List<User> GenerateTestUsers(int count = 5)
    {
        var users = new List<User>();
        
        for (int i = 1; i <= count; i++)
        {
            users.Add(new User
            {
                Id = Guid.NewGuid().ToString(),
                Username = $"testuser{i}@example.com",
                Email = $"testuser{i}@example.com",
                FirstName = $"FirstName{i}",
                LastName = $"LastName{i}",
                Password = "TestPassword123!",
                IsActive = i % 2 == 0, // Alternate active/inactive
                CreatedDate = DateTime.Now.AddDays(-i)
            });
        }
        
        return users;
    }

    public static List<Product> GenerateTestProducts(int count = 10)
    {
        var categories = new[] { "Electronics", "Clothing", "Books", "Sports", "Home" };
        var products = new List<Product>();
        
        for (int i = 1; i <= count; i++)
        {
            products.Add(new Product
            {
                Id = i.ToString(),
                Name = $"Product {i}",
                Description = $"Description for product {i}",
                Price = (decimal)(10.00 + (i * 5.50)),
                Category = categories[i % categories.Length],
                InStock = i % 3 != 0, // Most products in stock
                Quantity = i % 3 == 0 ? 0 : (10 + i)
            });
        }
        
        return products;
    }

    public static User GetValidTestUser()
    {
        return new User
        {
            Id = Guid.NewGuid().ToString(),
            Username = "validuser@example.com",
            Email = "validuser@example.com",
            FirstName = "Valid",
            LastName = "User",
            Password = "ValidPassword123!",
            IsActive = true,
            CreatedDate = DateTime.Now
        };
    }

    public static User GetInvalidTestUser()
    {
        return new User
        {
            Id = Guid.NewGuid().ToString(),
            Username = "invalid@example.com",
            Email = "invalid@example.com",
            FirstName = "Invalid",
            LastName = "User",
            Password = "WrongPassword",
            IsActive = false,
            CreatedDate = DateTime.Now
        };
    }

    public static Dictionary<string, string> GetTestCredentials()
    {
        return new Dictionary<string, string>
        {
            { "valid_user", "validuser@example.com" },
            { "valid_password", "ValidPassword123!" },
            { "invalid_user", "invalid@example.com" },
            { "invalid_password", "WrongPassword" },
            { "admin_user", "admin@example.com" },
            { "admin_password", "AdminPassword123!" }
        };
    }
}
