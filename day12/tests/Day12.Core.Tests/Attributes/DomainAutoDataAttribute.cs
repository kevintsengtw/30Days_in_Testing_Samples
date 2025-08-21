using AutoFixture;

namespace Day12.Core.Tests.Attributes;

/// <summary>
/// 領域物件自訂 AutoData 屬性
/// </summary>
public class DomainAutoDataAttribute : AutoDataAttribute
{
    public DomainAutoDataAttribute() : base(() => CreateFixture())
    {
    }

    internal static IFixture CreateFixture()
    {
        var fixture = new Fixture();

        // 設定 Person 的屬性
        fixture.Customize<Person>(composer => composer.With(p => p.Name, () => $"測試用戶{Random.Shared.Next(1, 999)}")
                                                      .With(p => p.Age, () => Random.Shared.Next(18, 65))
                                                      .With(p => p.Email, () => $"user{Random.Shared.Next(1, 999)}@example.com")
                                                      .With(p => p.CreateTime, DateTime.Now));

        // 設定 Product 的屬性
        fixture.Customize<Product>(composer => composer.With(p => p.Name, () => $"產品{Random.Shared.Next(100, 999)}")
                                                       .With(p => p.Price, () => Random.Shared.Next(100, 10000))
                                                       .With(p => p.IsAvailable, true)
                                                       .With(p => p.Description, () => $"這是測試產品的描述內容{Random.Shared.Next(1, 99)}"));

        return fixture;
    }
}