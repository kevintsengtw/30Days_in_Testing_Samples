using Day09.Core.Models;

namespace Day09.Core;

/// <summary>
/// class DataProcessor - 需要部分模擬的資料處理器
/// </summary>
public class DataProcessor
{
    /// <summary>
    /// 處理資料
    /// </summary>
    /// <param name="data">要處理的資料</param>
    /// <returns>處理結果</returns>
    public virtual ProcessResult ProcessData(string data)
    {
        var validationResult = ValidateData(data);
        if (!validationResult.IsValid)
        {
            return ProcessResult.Failed(validationResult.Errors);
        }

        var processedData = TransformData(data);
        var result = SaveData(processedData);

        return result;
    }

    /// <summary>
    /// 驗證資料
    /// </summary>
    /// <param name="data">要驗證的資料</param>
    /// <returns>驗證結果</returns>
    protected virtual ValidationResult ValidateData(string data)
    {
        // 複雜的驗證邏輯
        if (string.IsNullOrEmpty(data))
        {
            return new ValidationResult
            {
                IsValid = false,
                Errors = ["Data cannot be null or empty"]
            };
        }

        return new ValidationResult { IsValid = true };
    }

    /// <summary>
    /// 轉換資料
    /// </summary>
    /// <param name="data">要轉換的資料</param>
    /// <returns>轉換後的資料</returns>
    protected virtual string TransformData(string data)
    {
        // 複雜的轉換邏輯
        return data.ToUpper();
    }

    /// <summary>
    /// 儲存資料
    /// </summary>
    /// <param name="data">要儲存的資料</param>
    /// <returns>儲存結果</returns>
    protected virtual ProcessResult SaveData(string data)
    {
        // 實際的資料庫操作 - 在測試中需要模擬
        throw new NotImplementedException("Real database operation");
    }
}