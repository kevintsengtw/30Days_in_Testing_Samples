using System;

namespace Day03.Core.Tests.FixtureTests.Repositories;

/// <summary>
/// class UserRepositoryTests - 用於測試使用者資料庫存取的功能。
/// </summary>
public class UserRepositoryTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _databaseFixture;

    /// <summary>
    /// UserRepositoryTests 的建構函式
    /// </summary>
    /// <param name="databaseFixture">資料庫測試環境</param>
    public UserRepositoryTests(DatabaseFixture databaseFixture)
    {
        this._databaseFixture = databaseFixture;
    }
    
    [Fact]
    public void GetAllUsers_應回傳所有測試使用者()
    {
        // Act
        var users = this._databaseFixture.DbContext.Users.ToList();

        // Assert
        Assert.NotEmpty(users);
        Assert.True(users.Count >= 3); // 至少包含初始的 3 個使用者
        Assert.Contains(users, u => u.Name == "Test User 1");
        Assert.Contains(users, u => u.Name == "Test User 2");
        Assert.Contains(users, u => u.Name == "Admin User");
    }

    [Fact]
    public void GetUserByEmail_存在的Email_應回傳對應使用者()
    {
        // Arrange
        const string email = "test1@example.com";

        // Act
        var user = this._databaseFixture.DbContext.Users
                       .FirstOrDefault(u => u.Email == email);

        // Assert
        Assert.NotNull(user);
        Assert.Equal("Test User 1", user.Name);
        Assert.Equal(email, user.Email);
    }

    [Fact]
    public void CreateUser_新使用者_應成功建立()
    {
        // Arrange
        var newUser = new User
        {
            Name = "New Test User",
            Email = "newtest@example.com",
            Age = 28
        };

        // Act
        this._databaseFixture.DbContext.Users.Add(newUser);
        this._databaseFixture.DbContext.SaveChanges();

        // Assert
        Assert.True(newUser.Id > 0);

        // 驗證資料確實儲存到資料庫
        var retrievedUser = this._databaseFixture.DbContext.Users
                                .FirstOrDefault(u => u.Email == newUser.Email);
        
        Assert.NotNull(retrievedUser);
        Assert.Equal(newUser.Name, retrievedUser.Name);
    }
}