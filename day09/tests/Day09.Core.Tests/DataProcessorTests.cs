using System;
using Day09.Core.Models;

namespace Day09.Core.Tests;

/// <summary>
/// DataProcessor 測試類別（部分模擬測試）
/// </summary>
public class DataProcessorTests
{
    [Fact]
    public void ProcessData_正常資料_應成功處理()
    {
        // Arrange
        var processor = new TestableDataProcessor();
        var validData = "test data";

        // Act
        var actual = processor.ProcessData(validData);

        // Assert
        actual.IsSuccess.Should().BeTrue();
        actual.Errors.Should().BeEmpty();
    }

    [Fact]
    public void ProcessData_空資料_應回傳失敗結果()
    {
        // Arrange
        var processor = new DataProcessor();
        var invalidData = "";

        // Act
        var actual = processor.ProcessData(invalidData);

        // Assert
        actual.IsSuccess.Should().BeFalse();
        actual.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void ProcessData_Null資料_應回傳失敗結果()
    {
        // Arrange
        var processor = new DataProcessor();
        string? invalidData = null;

        // Act
        var actual = processor.ProcessData(invalidData!);

        // Assert
        actual.IsSuccess.Should().BeFalse();
        actual.Errors.Should().NotBeEmpty();
    }
}

/// <summary>
/// 可測試的 DataProcessor，覆寫 SaveData 避免實際資料庫操作
/// </summary>
public class TestableDataProcessor : DataProcessor
{
    protected override ProcessResult SaveData(string data)
    {
        // 模擬成功的儲存操作
        return ProcessResult.Success();
    }
}