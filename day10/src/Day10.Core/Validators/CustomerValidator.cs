using Day10.Core.Models;

namespace Day10.Core.Validators;

/// <summary>
/// 客戶驗證器
/// </summary>
public class CustomerValidator
{
    /// <summary>
    /// 驗證是否為成年人
    /// </summary>
    public bool IsAdult(Customer customer)
    {
        return customer.Age >= 18;
    }
}