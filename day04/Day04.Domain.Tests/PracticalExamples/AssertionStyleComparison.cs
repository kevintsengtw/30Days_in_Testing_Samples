using Day04.Domain.Models;
using Day04.Domain.Services;

namespace Day04.Domain.Tests.PracticalExamples;

/// <summary>
/// class AssertionStyleComparison - 斷言風格比較
/// </summary>
public class AssertionStyleComparison
{
    private readonly UserService _userService = new();

    [Fact]
    public void Assert_傳統與流暢風格比較_應展示語法差異()
    {
        var users = new List<User>
        {
            this._userService.CreateUser("user1@example.com", "User One"),
            this._userService.CreateUser("user2@example.com", "User Two")
        };

        // 傳統 Assert 風格（不推薦）
        /*
        Assert.NotNull(users);
        Assert.True(users.Count > 0);
        Assert.True(users.All(u => u.IsActive));
        Assert.Contains(users, u => u.Email.Contains("@example.com"));
        */

        // AwesomeAssertions 流暢風格（推薦）
        users.Should().NotBeNull()
             .And.NotBeEmpty()
             .And.AllSatisfy(u => u.IsActive.Should().BeTrue())
             .And.Contain(u => u.Email.Contains("@example.com"));

        // 優勢：更清楚的錯誤訊息、更好的可讀性、支援方法鏈結
    }

    [Fact]
    public void BeEquivalentTo_複雜物件比較_應展示流暢風格優勢()
    {
        var expectedUser = new User
        {
            Name = "John",
            Email = "john@example.com",
            Age = 30
        };
        var actualUser = this._userService.CreateUser("john@example.com", "John");
        actualUser.Age = 30;

        // 傳統風格需要多個斷言
        /*
        Assert.Equal(expectedUser.Name, actualUser.Name);
        Assert.Equal(expectedUser.Email, actualUser.Email);
        Assert.Equal(expectedUser.IsActive, actualUser.IsActive);
        */

        // 流暢風格：一行搞定，錯誤訊息更詳細
        actualUser.Should().BeEquivalentTo(expectedUser,
                                           options => options.Excluding(u => u.Id)
                                                             .Excluding(u => u.CreatedAt));
    }

    [Fact]
    public void Contain_錯誤訊息比較_應展示詳細程度差異()
    {
        var numbers = new[] { 1, 2, 3, 4, 5 };

        // 這個測試故意失敗來展示錯誤訊息的差異
        // 取消註解其中一行來看錯誤訊息

        // 傳統方式的錯誤訊息較簡單
        // Assert.Contains(10, numbers); // 錯誤訊息: Assert.Contains() Failure: Not found: 10

        // AwesomeAssertions 的錯誤訊息更詳細
        // numbers.Should().Contain(10); // 錯誤訊息會顯示實際的集合內容和期望值，錯誤訊息：Expected numbers {1, 2, 3, 4, 5} to contain 10.

        // 正確的斷言
        numbers.Should().Contain(3);
    }
}