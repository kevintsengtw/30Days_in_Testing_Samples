namespace Day16.Core.Tests;

/// <summary>
/// FakeTimeProvider 的 AutoFixture 自訂設定
/// </summary>
public class FakeTimeProviderCustomization : ICustomization
{
    /// <summary>
    /// 自訂 AutoFixture 設定
    /// </summary>
    /// <param name="fixture">Fixture 實例</param>
    public void Customize(IFixture fixture)
    {
        fixture.Register(() => new FakeTimeProvider());
    }
}

/// <summary>
/// 包含自訂設定的 AutoData 屬性
/// </summary>
public class AutoDataWithCustomizationAttribute : AutoDataAttribute
{
    /// <summary>
    /// 建立 AutoDataWithCustomization 屬性
    /// </summary>
    public AutoDataWithCustomizationAttribute() : base(CreateFixture)
    {
    }

    /// <summary>
    /// 建立自訂的 Fixture
    /// </summary>
    /// <returns>設定好的 Fixture 實例</returns>
    private static IFixture CreateFixture()
    {
        var fixture = new Fixture()
                      .Customize(new AutoNSubstituteCustomization())
                      .Customize(new FakeTimeProviderCustomization());

        return fixture;
    }
}

/// <summary>
/// 包含自訂設定的 InlineAutoData 屬性
/// </summary>
public class InlineAutoDataWithCustomizationAttribute : InlineAutoDataAttribute
{
    /// <summary>
    /// 建立 InlineAutoDataWithCustomization 屬性
    /// </summary>
    /// <param name="values">內嵌測試資料</param>
    public InlineAutoDataWithCustomizationAttribute(params object[] values)
        : base(new AutoDataWithCustomizationAttribute(), values)
    {
    }
}