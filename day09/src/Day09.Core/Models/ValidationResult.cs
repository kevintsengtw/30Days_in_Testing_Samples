using System;

namespace Day09.Core.Models;

/// <summary>
/// class ValidationResult - 驗證結果
/// </summary>
public class ValidationResult
{
    /// <summary>
    /// 是否有效
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// 錯誤訊息列表
    /// </summary>
    public List<string> Errors { get; set; } = new();
}