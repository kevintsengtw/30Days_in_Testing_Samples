namespace Day03.Core.Tests.TestDataProviders;

/// <summary>
/// class UserTestDataProvider - 提供使用者測試資料的實作。
/// </summary>
public class UserTestDataProvider : ITestDataProvider<User>
{
    /// <summary>
    /// 提供有效的使用者測試資料。
    /// </summary>
    /// <returns></returns>
    public IEnumerable<User> GetValidData()
    {
        yield return new User
        {
            Name = "John Doe",
            Email = "john@example.com",
            Age = 30,
            Roles = ["User"]
        };

        yield return new User
        {
            Name = "Admin User",
            Email = "admin@company.com",
            Age = 35,
            Roles = ["Admin", "User"]
        };
    }

    /// <summary>
    /// 提供無效的使用者測試資料。
    /// </summary>
    /// <returns></returns>
    public IEnumerable<User> GetInvalidData()
    {
        yield return new User
        {
            Name = "",
            Email = "john@example.com",
            Age = 30
        };

        yield return new User
        {
            Name = "John",
            Email = "invalid-email",
            Age = 30
        };
    }

    /// <summary>
    /// 提供邊界測試資料。
    /// </summary>
    /// <returns></returns>
    public IEnumerable<User> GetBoundaryData()
    {
        yield return new User
        {
            Name = "Young User",
            Email = "young@example.com",
            Age = 18 // 最小年齡
        };

        yield return new User
        {
            Name = "Old User",
            Email = "old@example.com",
            Age = 120 // 最大年齡
        };
    }

    /// <summary>
    /// 提供範例測試資料。
    /// </summary>
    /// <returns></returns>
    public User GetSampleData()
    {
        return new User
        {
            Name = "Sample User",
            Email = "sample@example.com",
            Age = 25,
            Roles = ["User"]
        };
    }
}