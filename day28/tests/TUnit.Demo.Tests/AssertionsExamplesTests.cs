namespace TUnit.Demo.Tests;

/// <summary>
/// TUnit Assertions - 範例測試
/// </summary>
public class AssertionsExamplesTests
{
    #region And 條件組合

    [Test]
    public async Task And條件組合範例()
    {
        var number = 10;

        // 組合多個條件
        await Assert.That(number)
                    .IsGreaterThan(5)
                    .And.IsLessThan(15)
                    .And.IsEqualTo(10);

        var email = "test@example.com";
        await Assert.That(email)
                    .Contains("@")
                    .And.EndsWith(".com")
                    .And.StartsWith("test");
    }

    #endregion

    #region Or 條件組合

    [Test]
    public async Task Or條件組合範例()
    {
        var number = 15;

        // 任一條件成立即可通過
        await Assert.That(number)
                    .IsEqualTo(10)
                    .Or.IsEqualTo(15)
                    .Or.IsEqualTo(20);

        var text = "Hello World";
        await Assert.That(text)
                    .StartsWith("Hi")
                    .Or.StartsWith("Hello")
                    .Or.StartsWith("Hey");
    }

    [Test]
    public async Task Or條件實務範例()
    {
        var email = "admin@company.com";

        // 檢查是否為管理員或測試帳號
        await Assert.That(email)
                    .StartsWith("admin@")
                    .Or.StartsWith("test@")
                    .Or.Contains("@localhost");

        var httpStatusCode = 200;

        // 檢查是否為成功的 HTTP 狀態碼
        await Assert.That(httpStatusCode)
                    .IsEqualTo(200)     // OK
                    .Or.IsEqualTo(201)  // Created
                    .Or.IsEqualTo(204); // No Content
    }

    #endregion

    #region 浮點數精確度控制

    [Test]
    [Arguments(3.14159, 3.14, 0.01)]
    [Arguments(1.0001, 1.0, 0.001)]
    [Arguments(99.999, 100.0, 0.01)]
    public async Task 浮點數精確度控制(double actual, double expected, double tolerance)
    {
        // 浮點數比較允許誤差範圍
        await Assert.That(actual)
                    .IsEqualTo(expected)
                    .Within(tolerance);
    }

    #endregion

    #region 集合斷言範例

    [Test]
    public async Task 集合斷言範例()
    {
        var numbers = new List<int> { 1, 2, 3, 4, 5 };
        var emptyList = new List<string>();

        // 集合計數檢查
        await Assert.That(numbers).HasCount(5);
        await Assert.That(emptyList).IsEmpty();
        await Assert.That(numbers).IsNotEmpty();

        // 元素包含檢查
        await Assert.That(numbers).Contains(3);
        await Assert.That(numbers).DoesNotContain(10);

        // 集合位置檢查
        await Assert.That(numbers.First()).IsEqualTo(1);
        await Assert.That(numbers.Last()).IsEqualTo(5);
        await Assert.That(numbers[2]).IsEqualTo(3);

        // 集合全部檢查
        await Assert.That(numbers.All(x => x > 0)).IsTrue();
        await Assert.That(numbers.Any(x => x > 3)).IsTrue();
    }

    #endregion

    #region 字串斷言範例

    [Test]
    public async Task 字串斷言範例()
    {
        var email = "user@example.com";
        var emptyString = "";

        // 包含檢查
        await Assert.That(email).Contains("@");
        await Assert.That(email).Contains("example");
        await Assert.That(email).DoesNotContain(" ");

        // 開始/結束檢查
        await Assert.That(email).StartsWith("user");
        await Assert.That(email).EndsWith(".com");

        // 空字串檢查
        await Assert.That(emptyString).IsEmpty();
        await Assert.That(email).IsNotEmpty();

        // 字串長度檢查
        await Assert.That(email.Length).IsGreaterThan(5);
        await Assert.That(email.Length).IsEqualTo(16);
    }

    #endregion
}