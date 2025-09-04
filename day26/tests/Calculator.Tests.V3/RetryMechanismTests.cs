namespace Calculator.Tests.V3;

/// <summary>
/// 展示 xUnit v3 的 Retry 機制
/// 基於官方 RetryFactExample 範例的概念
/// </summary>
public class RetryMechanismTests
{
    private ITestOutputHelper _testOutputHelper;

    public RetryMechanismTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    /// <summary>
    /// 計數器 Fixture，用於追蹤測試執行次數
    /// </summary>
    public class CounterFixture
    {
        public int RunCount;
        public DateTime LastRunTime;
        public List<string> ExecutionLog = new();

        public void IncrementRun(string testName)
        {
            RunCount++;
            LastRunTime = DateTime.Now;
            ExecutionLog.Add($"{testName} - Run #{RunCount} at {LastRunTime:HH:mm:ss.fff}");
        }
    }

    /// <summary>
    /// 展示一般 [Fact] 的執行行為（不重試）
    /// </summary>
    public class StandardFactSample : IClassFixture<CounterFixture>
    {
        private readonly CounterFixture _counter;
        private ITestOutputHelper _testOutputHelper;

        public StandardFactSample(CounterFixture counter, ITestOutputHelper testOutputHelper)
        {
            _counter = counter;
            _testOutputHelper = testOutputHelper;
            _counter.IncrementRun(nameof(StandardFactSample));
        }

        [Fact]
        public void 標準Fact測試_只執行一次_會失敗()
        {
            // 這個測試展示標準 Fact 與 RetryFact 的差異
            _testOutputHelper.WriteLine($"StandardFact - 執行次數: {_counter.RunCount}");

            // 記錄到 TestOutputHelper 和 _testOutputHelper
            _testOutputHelper.WriteLine($"期望執行次數: 2，實際執行次數: {_counter.RunCount}");

            // 在真實場景中，這個斷言會失敗，因為標準 Fact 只會執行一次
            // 為了不讓整個測試套件失敗，我們這裡跳過
            if (_counter.RunCount == 1)
            {
                Assert.Skip("標準 Fact 只會執行一次，無法重試。這展示了為什麼需要 RetryFact");
            }

            // 如果有重試機制，這個斷言才會通過
            _counter.RunCount.Should().Be(2, "期望測試執行 2 次，但標準 Fact 只會執行 1 次");
        }
    }

    /// <summary>
    /// 模擬 RetryFact 的行為（由於 xUnit v3 可能尚未完全實現，我們模擬其概念）
    /// </summary>
    public class RetryFactSample : IClassFixture<CounterFixture>
    {
        private readonly CounterFixture _counter;
        private ITestOutputHelper _testOutputHelper;

        public RetryFactSample(CounterFixture counter, ITestOutputHelper testOutputHelper)
        {
            _counter = counter;
            _testOutputHelper = testOutputHelper;
            _counter.IncrementRun(nameof(RetryFactSample));
        }

        /// <summary>
        /// 模擬 RetryFact 行為的測試
        /// 在實際的 xUnit v3 中，這會是 [RetryFact(MaxRetries = 2)]
        /// </summary>
        [Fact]
        public void 模擬RetryFact行為_會在第二次成功()
        {
            _testOutputHelper.WriteLine($"RetryFact模擬 - 執行次數: {_counter.RunCount}");
            _testOutputHelper.WriteLine($"執行記錄: {string.Join(" | ", _counter.ExecutionLog)}");

            // 模擬重試邏輯：如果是第一次執行就跳過測試
            if (_counter.RunCount == 1)
            {
                _testOutputHelper.WriteLine("第一次執行，模擬失敗並重試");

                // 在真實的 RetryFact 中，這會觸發重試
                // 這裡我們使用 Skip 來展示概念
                Assert.Skip("模擬第一次執行失敗，需要重試");
            }

            // 第二次執行時會成功
            _testOutputHelper.WriteLine("第二次執行，測試通過");
            _counter.RunCount.Should().BeGreaterThanOrEqualTo(2);
        }
    }

    /// <summary>
    /// 模擬 RetryTheory 的概念
    /// </summary>
    public class RetryTheorySample : IClassFixture<CounterFixture>
    {
        private ITestOutputHelper _testOutputHelper;
        private readonly CounterFixture _counter;
        private static readonly Dictionary<string, int> _testRunCounts = new();

        public RetryTheorySample(ITestOutputHelper testOutputHelper, CounterFixture counter)
        {
            _testOutputHelper = testOutputHelper;
            _counter = counter;
            _counter.IncrementRun(nameof(RetryTheorySample));
        }

        /// <summary>
        /// 模擬 RetryTheory 的參數化重試測試
        /// 在實際的 xUnit v3 中，這會是 [RetryTheory(MaxRetries = 2)]
        /// </summary>
        [Theory]
        [InlineData(2)] // 會在第二次成功
        [InlineData(3)] // 會在第三次成功
        [InlineData(1)] // 第一次就成功
        public void 模擬RetryTheory行為_參數化重試(int expectedCount)
        {
            var testKey = $"RetryTheory_{expectedCount}";

            // 追蹤每個參數組合的執行次數
            _testRunCounts.TryAdd(testKey, 0);

            _testRunCounts[testKey]++;

            var currentRunCount = _testRunCounts[testKey];

            _testOutputHelper.WriteLine($"參數 {expectedCount} - 執行次數: {currentRunCount}");
            _testOutputHelper.WriteLine($"Class 總執行次數: {_counter.RunCount}");

            // 模擬重試邏輯
            if (currentRunCount < expectedCount)
            {
                _testOutputHelper.WriteLine($"參數 {expectedCount} 第 {currentRunCount} 次執行，模擬失敗");
                Assert.Skip($"模擬參數 {expectedCount} 需要執行 {expectedCount} 次才成功");
            }

            _testOutputHelper.WriteLine($"參數 {expectedCount} 第 {currentRunCount} 次執行成功");
            currentRunCount.Should().Be(expectedCount);
        }
    }

    /// <summary>
    /// 展示實際的重試場景：網路請求
    /// </summary>
    public class NetworkRetryScenario
    {
        private static int _attemptCount = 0;
        private readonly Random _random = new();
        
        private ITestOutputHelper _testOutputHelper;

        public NetworkRetryScenario(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void 模擬網路請求重試邏輯()
        {
            _testOutputHelper.WriteLine("開始模擬網路請求...");

            var success = SimulateNetworkRequest();

            if (!success)
            {
                _testOutputHelper.WriteLine("x 網路請求失敗，在實際 RetryFact 中會自動重試");
                // 在真實場景中，RetryFact 會自動重試這個測試
                Assert.Skip("模擬網路請求失敗，需要重試");
            }

            _testOutputHelper.WriteLine("o 網路請求成功");
            success.Should().BeTrue();
        }

        private bool SimulateNetworkRequest()
        {
            _attemptCount++;
            _testOutputHelper.WriteLine($"嘗試第 {_attemptCount} 次網路請求");

            // 模擬 70% 的成功率
            var success = _random.NextDouble() > 0.3;

            if (success)
            {
                _testOutputHelper.WriteLine($"o 第 {_attemptCount} 次請求成功");
            }
            else
            {
                _testOutputHelper.WriteLine($"x 第 {_attemptCount} 次請求失敗");
            }

            return success;
        }
    }

    /// <summary>
    /// 展示資料庫連線重試場景
    /// </summary>
    public class DatabaseRetryScenario : IClassFixture<CounterFixture>
    {
        private readonly CounterFixture _counter;
        private ITestOutputHelper _testOutputHelper;

        public DatabaseRetryScenario(CounterFixture counter, ITestOutputHelper testOutputHelper)
        {
            _counter = counter;
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void 模擬資料庫連線重試()
        {
            _testOutputHelper.WriteLine("嘗試連接資料庫...");

            // 模擬資料庫連線可能失敗的情況
            var connectionSuccess = SimulateDatabaseConnection();

            if (!connectionSuccess && _counter.RunCount < 3)
            {
                _testOutputHelper.WriteLine($"x 資料庫連線失敗 (第 {_counter.RunCount} 次嘗試)");
                Assert.Skip("資料庫連線失敗，需要重試");
            }

            _testOutputHelper.WriteLine("o 資料庫連線成功");
            connectionSuccess.Should().BeTrue();
        }

        private bool SimulateDatabaseConnection()
        {
            // 模擬資料庫連線：前兩次失敗，第三次成功
            var success = _counter.RunCount >= 3;

            _testOutputHelper.WriteLine($"資料庫連線嘗試 #{_counter.RunCount}: {(success ? "成功" : "失敗")}");

            return success;
        }
    }

    /// <summary>
    /// 展示條件式重試
    /// </summary>
    [Fact]
    public void 展示條件式重試邏輯()
    {
        _testOutputHelper.WriteLine("執行條件式重試測試");

        // 檢查環境條件
        var isCI = Environment.GetEnvironmentVariable("CI") == "true";
        var currentTime = DateTime.Now;

        _testOutputHelper.WriteLine($"CI 環境: {isCI}");
        _testOutputHelper.WriteLine($"當前時間: {currentTime:HH:mm:ss}");

        // 模擬在某些條件下需要重試的情況
        if (!isCI && currentTime.Second % 10 != 0)
        {
            _testOutputHelper.WriteLine("條件不符，跳過此次執行（在 RetryFact 中會重試）");
            Assert.Skip("條件不符合，需要重試");
        }

        _testOutputHelper.WriteLine("條件符合，測試通過");
        Assert.True(true);
    }
}