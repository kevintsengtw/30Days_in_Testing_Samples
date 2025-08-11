using Day01.Core;

namespace Day01.Samples.Tests;

/// <summary>
/// class CounterTests - Counter 測試類別
/// </summary>
public class CounterTests
{
    //---------------------------------------------------------------------------------------------
    // 展示 Independent 和 Repeatable 原則

    [Fact] // Independent: 每個測試都建立新的實例
    public void Increment_從0開始_應回傳1()
    {
        // Arrange
        var counter = new Counter(); // 每個測試都建立新的實例

        // Act
        counter.Increment();

        // Assert
        Assert.Equal(1, counter.Value);
    }

    [Fact] // Independent: 不受其他測試影響
    public void Increment_從0開始連續兩次_應回傳2()
    {
        // Arrange
        var counter = new Counter(); // 獨立的實例，不受其他測試影響

        // Act
        counter.Increment();
        counter.Increment();

        // Assert
        Assert.Equal(2, counter.Value);
    }

    [Fact] // Repeatable: 每次執行都得到相同結果
    public void Increment_多次執行_應產生一致結果()
    {
        // Arrange
        var counter = new Counter();

        // Act
        counter.Increment();
        counter.Increment();
        counter.Increment();

        // Assert - 每次執行這個測試都會得到相同結果
        Assert.Equal(3, counter.Value);
    }

    //---------------------------------------------------------------------------------------------
    // Decrement 方法測試

    [Fact]
    public void Decrement_從0開始_應回傳負1()
    {
        // Arrange
        var counter = new Counter();

        // Act
        counter.Decrement();

        // Assert
        Assert.Equal(-1, counter.Value);
    }

    [Fact]
    public void Decrement_從正數開始_應正確減少()
    {
        // Arrange
        var counter = new Counter();
        counter.SetValue(5);

        // Act
        counter.Decrement();

        // Assert
        Assert.Equal(4, counter.Value);
    }

    //---------------------------------------------------------------------------------------------
    // Reset 方法測試

    [Fact]
    public void Reset_從任意值_應回傳0()
    {
        // Arrange
        var counter = new Counter();
        counter.SetValue(100);

        // Act
        counter.Reset();

        // Assert
        Assert.Equal(0, counter.Value);
    }

    //---------------------------------------------------------------------------------------------
    // SetValue 方法測試

    [Theory]
    [InlineData(10)]
    [InlineData(-5)]
    [InlineData(0)]
    [InlineData(999)]
    public void SetValue_輸入任意值_應設定正確數值(int value)
    {
        // Arrange
        var counter = new Counter();

        // Act
        counter.SetValue(value);

        // Assert
        Assert.Equal(value, counter.Value);
    }

    //---------------------------------------------------------------------------------------------
    // 複合操作測試

    public void 複合操作_增加減少重設_應產生預期結果()
    {
        // Arrange
        var counter = new Counter();

        // Act & Assert 組合 - 展示連續操作的獨立性
        counter.Increment();
        Assert.Equal(1, counter.Value);

        counter.Increment();
        Assert.Equal(2, counter.Value);

        counter.Decrement();
        Assert.Equal(1, counter.Value);

        counter.Reset();
        Assert.Equal(0, counter.Value);
    }
}