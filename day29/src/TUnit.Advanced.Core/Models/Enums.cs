namespace TUnit.Advanced.Core.Models;

/// <summary>
/// 訂單狀態列舉
/// </summary>
public enum OrderStatus
{
    待處理,
    已確認,
    處理中,
    已發貨,
    已完成,
    已取消
}

/// <summary>
/// 折扣類型列舉
/// </summary>
public enum DiscountType
{
    無折扣,
    百分比折扣,
    固定金額折扣,
    滿額折扣
}

/// <summary>
/// 客戶等級
/// </summary>
public enum CustomerLevel
{
    一般會員,
    VIP會員,
    白金會員,
    鑽石會員
}