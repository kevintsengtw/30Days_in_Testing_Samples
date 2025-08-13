using System.Net;
using AwesomeAssertions.Execution;
using Day04.Domain.Models;
using Day04.Domain.Services;

namespace Day04.Domain.Tests.PracticalExamples;

/// <summary>
/// class DomainSpecificAssertionPatterns - 領域特定斷言模式
/// </summary>
public class DomainSpecificAssertionPatterns
{
    private readonly UserService _userService = new();

    [Fact]
    public void CreateUser_有效使用者資料_應符合業務規則()
    {
        var user = this._userService.CreateUser("john@example.com", "John Doe");

        // 業務規則驗證模式
        user.Should().NotBeNull();

        user.Email.Should().MatchRegex(@"^[^@]+@[^@]+\.[^@]+$", "因為電子郵件應符合有效格式");

        user.Name.Should().NotBeNullOrWhiteSpace()
            .And.Match(name => name.Length >= 2 && name.Length <= 50, "因為姓名長度應介於2到50個字元之間");

        user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1), "因為使用者建立時間應為近期");
    }

    [Fact]
    public void ApiResponse_正常回應_應符合API規格()
    {
        var response = new ApiResponse<UserProfile>
        {
            StatusCode = HttpStatusCode.OK,
            Data = new UserProfile { Age = 30, City = "New York" },
            ResponseTime = TimeSpan.FromMilliseconds(500)
        };

        // API 回應驗證模式
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Data.Should().NotBeNull()
                .And.BeOfType<UserProfile>();

        // 回應時間驗證
        response.ResponseTime.Should().BeLessThan(TimeSpan.FromSeconds(2), "因為API回應時間應在可接受範圍內");
    }

    [Fact]
    public void CreateUser_複雜業務規則_應使用AssertionScope驗證()
    {
        var user = this._userService.CreateUser("premium@example.com", "Premium User");
        user.Age = 25;

        // 複合業務規則驗證
        using (new AssertionScope())
        {
            user.Email.Should().Contain("@", "因為有效的電子郵件必須包含 @ 符號");
            user.Age.Should().BeInRange(18, 120, "因為年齡必須在合理範圍內");
            user.Name.Should().NotContain("admin", "因為使用者名稱不應包含admin");
            user.IsActive.Should().BeTrue("因為新使用者預設應為啟用狀態");
        }

        // AssertionScope 的用意是讓多個斷言（Assertions）可以在同一區塊內執行，
        // 即使其中有斷言失敗，也會繼續執行後續斷言，最後一次性彙報所有失敗訊息。
        // 這樣可以讓測試結果更完整，方便一次檢查多個條件，提升除錯效率。      

        // 簡單來說，AssertionScope 讓你在一個範圍內收集多個驗證失敗訊息，而不是遇到第一個失敗就中斷測試。
    }
}