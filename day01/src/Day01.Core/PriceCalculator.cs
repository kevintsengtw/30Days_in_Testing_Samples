namespace Day01.Core;

/// <summary>
/// class PriceCalculator
/// </summary>
public class PriceCalculator
{
    /// <summary>
    /// 計算折扣後的價格
    /// </summary>
    /// <param name="basePrice">基礎價格</param>
    /// <param name="discount">折扣比例 (0.0 到 1.0 之間)</param>
    /// <returns>折扣後的價格</returns>
    /// <exception cref="ArgumentException">當價格為負數或折扣比例無效時拋出</exception>
    public decimal Calculate(decimal basePrice, decimal discount)
    {
        if (basePrice < 0)
        {
            throw new ArgumentException("基礎價格不能為負數", nameof(basePrice));
        }

        if (discount is < 0 or > 1)
        {
            throw new ArgumentException("折扣比例必須在 0 到 1 之間", nameof(discount));
        }

        return basePrice * (1 - discount);
    }

    /// <summary>
    /// 計算含稅價格
    /// </summary>
    /// <param name="basePrice">基礎價格</param>
    /// <param name="taxRate">稅率</param>
    /// <returns>含稅價格</returns>
    public decimal CalculateWithTax(decimal basePrice, decimal taxRate)
    {
        if (basePrice < 0)
        {
            throw new ArgumentException("基礎價格不能為負數", nameof(basePrice));
        }

        if (taxRate < 0)
        {
            throw new ArgumentException("稅率不能為負數", nameof(taxRate));
        }

        return basePrice * (1 + taxRate);
    }
}