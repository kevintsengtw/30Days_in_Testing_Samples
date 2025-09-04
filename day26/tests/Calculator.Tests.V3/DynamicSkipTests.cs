using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Calculator.Tests.V3;

/// <summary>
/// 展示動態跳過測試功能
/// </summary>
public class DynamicSkipTests
{
    private readonly Core.Calculator _calculator;

    public DynamicSkipTests()
    {
        _calculator = new Core.Calculator();
    }

    /// <summary>
    /// 展示 SkipUnless 屬性的用法
    /// </summary>
    [Fact(SkipUnless = nameof(IsWindowsEnvironment), Skip = "此測試只在 Windows 環境執行")]
    public void SkipUnless屬性測試()
    {
        // Act & Assert
        var result = _calculator.Add(5, 3);
        result.Should().Be(8);
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows).Should().BeTrue();
    }

    /// <summary>
    /// 判斷是否為 Windows 環境的靜態方法
    /// </summary>
    public static bool IsWindowsEnvironment =>
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

    /// <summary>
    /// 展示傳統方式的平台檢查（使用 Assert.Skip）
    /// </summary>
    [Fact]
    public void 傳統平台檢查測試()
    {
        // Arrange
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Assert.Skip("此測試只在 Windows 平台執行");
        }

        // Act & Assert
        var result = _calculator.Divide(100, 4);
        result.Should().Be(25);
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows).Should().BeTrue();
    }

    /// <summary>
    /// 展示根據條件動態跳過的測試
    /// </summary>
    [Fact]
    public void 根據條件動態跳過的測試()
    {
        // Arrange
        var isNewEngineEnabled = false; // 模擬新功能開關

        if (!isNewEngineEnabled)
        {
            Assert.Skip("新計算引擎功能尚未啟用，跳過此測試");
        }

        // Act
        var result = _calculator.Add(5, 3);

        // Assert
        result.Should().Be(8);
    }

    /// <summary>
    /// 展示環境變數控制的動態跳過
    /// </summary>
    [Fact]
    public void 根據環境變數跳過的測試()
    {
        // Arrange
        var enableIntegrationTests = Environment.GetEnvironmentVariable("ENABLE_INTEGRATION_TESTS");

        if (string.IsNullOrEmpty(enableIntegrationTests) || enableIntegrationTests.ToLower() != "true")
        {
            Assert.Skip("整合測試已停用。設定 ENABLE_INTEGRATION_TESTS=true 來執行此測試");
        }

        // Act & Assert
        var result = _calculator.Multiply(4, 6);
        result.Should().Be(24);
    }

    /// <summary>
    /// 展示功能開關控制的測試
    /// </summary>
    [Theory]
    [InlineData("FEATURE_ADVANCED_MATH")]
    [InlineData("FEATURE_STATISTICS")]
    public void 功能開關控制的測試(string featureName)
    {
        // Arrange
        var isFeatureEnabled = Environment.GetEnvironmentVariable(featureName)?.ToLower() == "true";

        if (!isFeatureEnabled)
        {
            Assert.Skip($"功能 {featureName} 尚未啟用");
        }

        // Act & Assert
        var result = _calculator.Add(1, 1);
        result.Should().Be(2);
    }

    /// <summary>
    /// 展示效能測試的動態跳過
    /// </summary>
    [Fact]
    public void 效能測試_根據環境決定是否執行()
    {
        // Arrange
        var enablePerformanceTests = Environment.GetEnvironmentVariable("ENABLE_PERFORMANCE_TESTS");

        if (string.IsNullOrEmpty(enablePerformanceTests) || enablePerformanceTests.ToLower() != "true")
        {
            Assert.Skip("效能測試已停用。設定 ENABLE_PERFORMANCE_TESTS=true 來執行效能測試");
        }

        // Act
        var startTime = DateTime.Now;
        for (var i = 0; i < 10000; i++)
        {
            _calculator.Add(i, i + 1);
        }

        var endTime = DateTime.Now;
        var duration = endTime - startTime;

        // Assert
        duration.TotalMilliseconds.Should().BeLessThan(1000);
    }

    /// <summary>
    /// 展示基於版本的動態跳過
    /// </summary>
    [Fact]
    public void NET版本檢查測試()
    {
        // Arrange
        var version = Environment.Version;

        if (version.Major < 8)
        {
            Assert.Skip($"此測試需要 .NET 8 或更新版本，目前版本: {version}");
        }

        // Act & Assert
        // 測試需要 .NET 8+ 功能的程式碼
        var result = _calculator.Multiply(8, 8);
        result.Should().Be(64);

        // 驗證版本
        version.Major.Should().BeGreaterThanOrEqualTo(8);
    }

    /// <summary>
    /// 展示資源可用性檢查
    /// </summary>
    [Theory]
    [InlineData("C:\\Windows\\System32\\notepad.exe")]
    public void 檔案存在性測試(string filePath)
    {
        // Arrange
        if (!File.Exists(filePath))
        {
            Assert.Skip($"測試需要的檔案不存在: {filePath}");
        }

        // Act & Assert
        var fileInfo = new FileInfo(filePath);
        fileInfo.Exists.Should().BeTrue();
        fileInfo.Length.Should().BeGreaterThan(0);
    }

    /// <summary>
    /// 展示記憶體需求檢查
    /// </summary>
    [Fact]
    public void 高記憶體需求測試()
    {
        // Arrange
        var availableMemory = GC.GetTotalMemory(false);
        var requiredMemory = 100 * 1024 * 1024; // 100MB

        if (availableMemory < requiredMemory)
        {
            Assert.Skip($"測試需要至少 {requiredMemory / (1024 * 1024)}MB 記憶體，目前可用: {availableMemory / (1024 * 1024)}MB");
        }

        // Act & Assert
        // 執行需要大量記憶體的測試
        var result = _calculator.Add(1000, 2000);
        result.Should().Be(3000);
    }

    /// <summary>
    /// 展示時間相關的動態跳過
    /// </summary>
    [Fact]
    public void 工作日測試()
    {
        // Arrange
        var today = DateTime.Now.DayOfWeek;

        if (today is DayOfWeek.Saturday or DayOfWeek.Sunday)
        {
            Assert.Skip("此測試只在工作日執行");
        }

        // Act & Assert
        var result = _calculator.Subtract(10, 3);
        result.Should().Be(7);
        today.Should().NotBe(DayOfWeek.Saturday);
        today.Should().NotBe(DayOfWeek.Sunday);
    }

    /// <summary>
    /// 展示組合條件的動態跳過
    /// </summary>
    [Fact]
    public void 組合條件測試()
    {
        // Arrange
        var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        var isDebugMode = Debugger.IsAttached;
        var hasEnvironmentVariable = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("TEST_MODE"));

        if (!isWindows || !isDebugMode || !hasEnvironmentVariable)
        {
            var skipReasons = new List<string>();
            if (!isWindows)
            {
                skipReasons.Add("需要 Windows 平台");
            }

            if (!isDebugMode)
            {
                skipReasons.Add("需要偵錯模式");
            }

            if (!hasEnvironmentVariable)
            {
                skipReasons.Add("需要設定 TEST_MODE 環境變數");
            }

            Assert.Skip($"跳過原因: {string.Join(", ", skipReasons)}");
        }

        // Act & Assert
        var result = _calculator.Multiply(6, 7);
        result.Should().Be(42);
    }
}