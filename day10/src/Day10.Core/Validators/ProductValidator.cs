namespace Day10.Core.Validators;

/// <summary>
/// 產品驗證器
/// </summary>
public class ProductValidator
{
    /// <summary>
    /// 驗證產品
    /// </summary>
    public bool Validate(object product)
    {
        // 簡單的驗證邏輯
        return product is not null;
    }
}