using AutoFixture;
using Bogus;
using Day15.TestLibrary.TestData.SpecimenBuilders;

namespace Day15.TestLibrary.TestData.Extensions;

/// <summary>
/// 設定 AutoFixture 使用 Bogus 整合
/// </summary>
public static class FixtureExtensions
{
    /// <summary>
    /// 為 AutoFixture 加入 Bogus 整合功能
    /// </summary>
    public static IFixture WithBogus(this IFixture fixture)
    {
        // 先設定循環參考處理
        fixture.WithOmitOnRecursion();

        // 先加入屬性層級的整合
        fixture.Customizations.Add(new EmailSpecimenBuilder());
        fixture.Customizations.Add(new PhoneSpecimenBuilder());
        fixture.Customizations.Add(new NameSpecimenBuilder());
        fixture.Customizations.Add(new AddressSpecimenBuilder());
        fixture.Customizations.Add(new WebsiteSpecimenBuilder());
        fixture.Customizations.Add(new CompanyNameSpecimenBuilder());

        // 再加入類型層級的整合
        fixture.Customizations.Add(new BogusSpecimenBuilder());

        return fixture;
    }

    /// <summary>
    /// 為特定類型註冊 Bogus Faker
    /// </summary>
    public static IFixture WithBogusFor<T>(this IFixture fixture, Faker<T> faker)
        where T : class
    {
        fixture.Customizations.Add(new TypedBogusSpecimenBuilder<T>(faker));
        return fixture;
    }

    /// <summary>
    /// 設定 AutoFixture 的循環參考處理
    /// </summary>
    public static IFixture WithOmitOnRecursion(this IFixture fixture)
    {
        fixture.Behaviors.OfType<ThrowingRecursionBehavior>()
               .ToList()
               .ForEach(b => fixture.Behaviors.Remove(b));
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        return fixture;
    }

    /// <summary>
    /// 設定集合的預設長度
    /// </summary>
    public static IFixture WithRepeatCount(this IFixture fixture, int count)
    {
        fixture.RepeatCount = count;
        return fixture;
    }

    /// <summary>
    /// 設定隨機種子以確保測試可重現性
    /// </summary>
    public static IFixture WithSeed(this IFixture fixture, int seed)
    {
        var random = new Random(seed);
        fixture.Register(() => random);
        Randomizer.Seed = new Random(seed);

        return fixture;
    }
}