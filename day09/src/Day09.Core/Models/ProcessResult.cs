using System;

namespace Day09.Core.Models;

/// <summary>
/// class ProcessResult - 處理結果
/// </summary>
public class ProcessResult
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// 錯誤訊息列表
    /// </summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// 建立失敗結果
    /// </summary>
    /// <param name="errors">錯誤訊息列表</param>
    /// <returns>失敗的處理結果</returns>
    public static ProcessResult Failed(List<string> errors)
    {
        return new ProcessResult
        {
            IsSuccess = false,
            Errors = errors
        };
    }

    /// <summary>
    /// 建立成功結果
    /// </summary>
    /// <returns>成功的處理結果</returns>
    public static ProcessResult Success()
    {
        return new ProcessResult { IsSuccess = true };
    }
}