namespace Day10.Core.Tests;

/// <summary>
/// AutoFixture 測試基礎類別，提供共同設定
/// </summary>
public abstract class AutoFixtureTestBase
{
    /// <summary>
    /// 建立已配置的 Fixture，處理循環參考問題
    /// </summary>
    protected Fixture CreateFixture()
    {
        var fixture = new Fixture();

        // 移除預設的循環參考拋出行為
        fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
               .ForEach(b => fixture.Behaviors.Remove(b));

        // 加入忽略循環參考的行為
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        return fixture;
    }

    /// <summary>
    /// 建立預設行為的 Fixture（會拋出循環參考例外）
    /// </summary>
    protected Fixture CreateDefaultFixture()
    {
        return new Fixture();
    }
}