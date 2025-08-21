using AutoFixture;

namespace Day12.Core.Tests.Attributes;

/// <summary>
/// 業務邏輯自訂 AutoData 屬性
/// </summary>
public class BusinessAutoDataAttribute : AutoDataAttribute
{
    public BusinessAutoDataAttribute() : base(() => CreateFixture())
    {
    }

    internal static IFixture CreateFixture()
    {
        var fixture = new Fixture();

        // 設定 Order 的屬性
        fixture.Customize<Order>(composer => composer.With(o => o.Status, OrderStatus.Created)
                                                     .With(o => o.Amount, () => Random.Shared.Next(1000, 50000))
                                                     .With(o => o.OrderNumber, () => $"ORD{DateTime.Now:yyyyMMdd}{Random.Shared.Next(1000, 9999)}"));

        return fixture;
    }
}