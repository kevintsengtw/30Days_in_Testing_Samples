namespace Day04.Domain.Tests.BasicAssertions;

/// <summary>
/// class StringAssertionTests - 字串斷言測試
/// </summary>
public class StringAssertionTests
{
    [Theory]
    [InlineData("Hello World", "Hello", "World")]
    [InlineData("Test@Example.com", "Test", ".com")]
    public void Should_StartWith_EndWith_Contain_有效字串_應正確驗證內容(string input, string start, string end)
    {
        input.Should().NotBeNullOrEmpty()
             .And.StartWith(start)
             .And.EndWith(end);

        // 通常不會在測試程式碼裡寫 if 判斷式
        if (input.Contains('@'))
        {
            input.Should().Contain("@");
        }
    }

    [Fact]
    public void MatchRegex_電子郵件和電話格式_應符合正規表達式()
    {
        const string email = "user@example.com";
        const string phoneNumber = "+1-555-123-4567";

        // 正規表達式斷言
        email.Should().MatchRegex(@"^[^@]+@[^@]+\.[^@]+$");

        // 格式斷言
        phoneNumber.Should().StartWith("+")
                   .And.Contain("-")
                   .And.HaveLength(15);
    }

    [Fact]
    public void BeEquivalentTo_忽略大小寫_應視為相等()
    {
        const string text = "Hello World";

        // 忽略大小寫的比較
        text.Should().BeEquivalentTo("hello world");
        text.ToLower().Should().StartWith("hello", "因為我們要忽略大小寫");
    }
}