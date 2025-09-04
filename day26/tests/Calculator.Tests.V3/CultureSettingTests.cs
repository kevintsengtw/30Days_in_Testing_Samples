using System.Globalization;

namespace Calculator.Tests.V3;

/// <summary>
/// 展示 xUnit 3.x 的 Culture 設定功能
/// 支援多語系測試場景，確保應用程式在不同文化環境下的正確性
/// </summary>
public class CultureSettingTests
{
    /// <summary>
    /// 測試英文文化下的貨幣格式
    /// </summary>
    [Fact]
    public void 使用英文文化的貨幣格式測試()
    {
        // Arrange
        var originalCulture = Thread.CurrentThread.CurrentCulture;
        var testValue = 123.45m;

        try
        {
            // Act
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            var result = testValue.ToString("C");

            // Assert
            result.Should().Be("$123.45");
        }
        finally
        {
            Thread.CurrentThread.CurrentCulture = originalCulture;
        }
    }

    /// <summary>
    /// 測試繁體中文文化下的日期格式
    /// </summary>
    [Fact]
    public void 使用繁體中文文化的日期格式測試()
    {
        // Arrange
        var originalCulture = Thread.CurrentThread.CurrentCulture;
        var testDate = new DateTime(2024, 12, 31);

        try
        {
            // Act
            Thread.CurrentThread.CurrentCulture = new CultureInfo("zh-TW");
            var result = testDate.ToString("yyyy年MM月dd日");

            // Assert
            result.Should().MatchRegex(@"\d{4}年\d{2}月\d{2}日");
            result.Should().Be("2024年12月31日");
        }
        finally
        {
            Thread.CurrentThread.CurrentCulture = originalCulture;
        }
    }

    /// <summary>
    /// 測試不同文化下的貨幣格式顯示
    /// </summary>
    [Theory]
    [InlineData("en-US")]
    [InlineData("ja-JP")]
    [InlineData("de-DE")]
    public void 多文化貨幣格式測試(string cultureName)
    {
        // Arrange
        var originalCulture = Thread.CurrentThread.CurrentCulture;
        var testValue = 123.45m;

        try
        {
            // Act
            Thread.CurrentThread.CurrentCulture = new CultureInfo(cultureName);
            var result = testValue.ToString("C");

            // Assert - 確認格式包含正確的貨幣符號和數值
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain("123");

            // 驗證特定文化的貨幣符號
            if (cultureName == "en-US")
            {
                result.Should().Contain("$");
            }
            else if (cultureName == "ja-JP")
            {
                // 日文可能使用全形或半形日圓符號
                result.Should().Match(s => s.Contains("¥") || s.Contains("￥"));
            }
            else if (cultureName == "de-DE")
            {
                result.Should().Contain("€");
            }
        }
        finally
        {
            Thread.CurrentThread.CurrentCulture = originalCulture;
        }
    }

    /// <summary>
    /// 測試不同文化下的數字格式
    /// </summary>
    [Theory]
    [InlineData("en-US")]
    [InlineData("de-DE")]
    [InlineData("fr-FR")]
    public void 多文化數字格式測試(string cultureName)
    {
        // Arrange
        var originalCulture = Thread.CurrentThread.CurrentCulture;
        var testValue = 1234.56;

        try
        {
            // Act
            Thread.CurrentThread.CurrentCulture = new CultureInfo(cultureName);
            var result = testValue.ToString("N2"); // 明確指定 2 位小數

            // Assert - 檢查基本格式特性
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain("234"); // 所有格式都包含 234
            result.Should().Contain("56");  // 所有格式都包含 56

            // 檢查是否包含正確的小數分隔符
            var culture = new CultureInfo(cultureName);
            var decimalSeparator = culture.NumberFormat.NumberDecimalSeparator;
            result.Should().Contain(decimalSeparator);

            // 驗證格式符合該文化的慣例
            if (cultureName == "en-US")
            {
                result.Should().MatchRegex(@"1,234\.56");
            }
            else if (cultureName == "de-DE")
            {
                result.Should().MatchRegex(@"1\.234,56");
            }
            else if (cultureName == "fr-FR")
            {
                // 法文格式可能因系統而異，檢查基本特徵
                result.Should().Match(s => s.Contains("234,56"));
            }
        }
        finally
        {
            Thread.CurrentThread.CurrentCulture = originalCulture;
        }
    }

    /// <summary>
    /// 測試使用文化敏感的字串比較
    /// </summary>
    [Fact]
    public void 文化敏感字串比較測試()
    {
        // Arrange
        var originalCulture = Thread.CurrentThread.CurrentCulture;
        var text1 = "Müller";
        var text2 = "Mueller";

        try
        {
            // Act & Assert - 德文文化下的比較
            Thread.CurrentThread.CurrentCulture = new CultureInfo("de-DE");
            var germanComparison = string.Compare(text1, text2, StringComparison.CurrentCulture);

            // 在德文文化中，ü 和 ue 可能被視為等價
            germanComparison.Should().NotBe(0, "在德文文化中，Müller 和 Mueller 應該被視為不同");

            // 使用不變文化比較
            var invariantComparison = string.Compare(text1, text2, StringComparison.InvariantCulture);
            invariantComparison.Should().NotBe(0, "在不變文化中，Müller 和 Mueller 應該被視為不同");
        }
        finally
        {
            Thread.CurrentThread.CurrentCulture = originalCulture;
        }
    }

    /// <summary>
    /// 測試文化無關的計算機功能
    /// 確保核心業務邏輯不受文化設定影響
    /// </summary>
    [Theory]
    [InlineData("en-US")]
    [InlineData("zh-TW")]
    [InlineData("de-DE")]
    [InlineData("fr-FR")]
    public void 計算機功能在不同文化下保持一致(string cultureName)
    {
        // Arrange
        var originalCulture = Thread.CurrentThread.CurrentCulture;
        var calculator = new Core.Calculator();

        try
        {
            // Act
            Thread.CurrentThread.CurrentCulture = new CultureInfo(cultureName);
            var result = calculator.Add(5, 3);

            // Assert - 計算結果不應受文化影響
            result.Should().Be(8);
        }
        finally
        {
            Thread.CurrentThread.CurrentCulture = originalCulture;
        }
    }

    /// <summary>
    /// 測試日期解析在不同文化下的行為
    /// </summary>
    [Theory]
    [InlineData("en-US", "12/31/2024")]
    [InlineData("en-GB", "31/12/2024")]
    [InlineData("zh-TW", "2024/12/31")]
    public void 日期解析文化測試(string cultureName, string dateString)
    {
        // Arrange
        var originalCulture = Thread.CurrentThread.CurrentCulture;
        var expectedDate = new DateTime(2024, 12, 31);

        try
        {
            // Act
            Thread.CurrentThread.CurrentCulture = new CultureInfo(cultureName);
            var parseSuccess = DateTime.TryParse(dateString, out var parsedDate);

            // Assert
            parseSuccess.Should().BeTrue($"應該能夠在 {cultureName} 文化下解析日期字串 '{dateString}'");
            parsedDate.Should().Be(expectedDate);
        }
        finally
        {
            Thread.CurrentThread.CurrentCulture = originalCulture;
        }
    }

    /// <summary>
    /// 測試 Calculator 的 ToString 方法在不同文化下的表現
    /// </summary>
    [Fact]
    public void Calculator結果ToString文化測試()
    {
        // Arrange
        var originalCulture = Thread.CurrentThread.CurrentCulture;
        var calculator = new Core.Calculator();

        try
        {
            // 測試英文文化
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            var result1 = calculator.Divide(10, 3);
            var stringResult1 = result1.ToString("F2");
            stringResult1.Should().Be("3.33");

            // 測試德文文化（使用逗號作為小數分隔符）
            Thread.CurrentThread.CurrentCulture = new CultureInfo("de-DE");
            var result2 = calculator.Divide(10, 3);
            var stringResult2 = result2.ToString("F2");
            stringResult2.Should().Be("3,33");
        }
        finally
        {
            Thread.CurrentThread.CurrentCulture = originalCulture;
        }
    }
}