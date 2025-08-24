using AutoFixture;
using Bogus;
using Day15.TestLibrary.TestData.SpecimenBuilders;

namespace Day15.TestLibrary.TestData;

/// <summary>
/// 混合資料產生器實作
/// </summary>
public class HybridTestDataGenerator : ITestDataGenerator
{
    private readonly IFixture _fixture;

    /// <summary>
    /// Initializes a new instance of the <see cref="HybridTestDataGenerator"/> class
    /// </summary>
    /// <param name="seed">The seed</param>
    public HybridTestDataGenerator(int? seed = null)
    {
        this._fixture = new Fixture();

        // 設定 Seed 以確保測試可重現性
        if (seed.HasValue)
        {
            this.SetSeed(seed.Value);
        }

        // 設定 AutoFixture 的預設行為
        this.ConfigureAutoFixture();

        // 整合 Bogus 到 AutoFixture
        this.IntegrateBogus();
    }

    /// <summary>
    /// 產生指定型別的單一物件
    /// </summary>
    /// <typeparam name="T">要產生的型別</typeparam>
    /// <returns>產生的物件</returns>
    public T Generate<T>()
    {
        return this._fixture.Create<T>();
    }

    /// <summary>
    /// 產生指定數量的物件集合
    /// </summary>
    /// <typeparam name="T">要產生的型別</typeparam>
    /// <param name="count">要產生的數量</param>
    /// <returns>產生的物件集合</returns>
    public IEnumerable<T> Generate<T>(int count)
    {
        return Enumerable.Range(0, count).Select(_ => this.Generate<T>());
    }

    /// <summary>
    /// 產生物件並可自訂產生後的處理
    /// </summary>
    /// <typeparam name="T">要產生的型別</typeparam>
    /// <param name="configure">自訂產生後的處理</param>
    /// <returns>產生的物件</returns>
    public T Generate<T>(Action<T> configure)
    {
        var item = this.Generate<T>();
        configure(item);
        return item;
    }

    /// <summary>
    /// 以指定建構子參數產生物件
    /// </summary>
    /// <typeparam name="T">要產生的型別</typeparam>
    /// <param name="constructorParameters">建構子參數</param>
    /// <returns>產生的物件</returns>
    public T Generate<T>(params object[] constructorParameters)
    {
        if (constructorParameters.Length == 0)
        {
            return this.Generate<T>();
        }

        return this._fixture.Build<T>()
                   .FromFactory(() => (T)Activator.CreateInstance(typeof(T), constructorParameters)!)
                   .Create();
    }

    /// <summary>
    /// 取得底層的 AutoFixture 實例，供進階使用
    /// </summary>
    public IFixture GetFixture()
    {
        return this._fixture;
    }

    /// <summary>
    /// 設定隨機種子，確保資料產生可重現
    /// </summary>
    /// <param name="seed">Seed</param>
    private void SetSeed(int seed)
    {
        // 設定 AutoFixture 的隨機種子
        var random = new Random(seed);
        this._fixture.Register(() => random);

        // 設定 Bogus 的隨機種子（稍後在 SpecimenBuilder 中使用）
        Randomizer.Seed = new Random(seed);
    }

    /// <summary>
    /// 設定 AutoFixture 的預設行為
    /// </summary>
    private void ConfigureAutoFixture()
    {
        // 循環參考處理
        this._fixture.Behaviors.OfType<ThrowingRecursionBehavior>()
            .ToList()
            .ForEach(b => this._fixture.Behaviors.Remove(b));
        this._fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        // 設定集合長度
        this._fixture.RepeatCount = 3;
    }

    /// <summary>
    /// 整合 Bogus 與 AutoFixture 的自訂 SpecimenBuilder
    /// </summary>
    private void IntegrateBogus()
    {
        // 先加入屬性層級的整合（優先級較高）
        this._fixture.Customizations.Add(new EmailSpecimenBuilder());
        this._fixture.Customizations.Add(new PhoneSpecimenBuilder());
        this._fixture.Customizations.Add(new NameSpecimenBuilder());
        this._fixture.Customizations.Add(new AddressSpecimenBuilder());
        this._fixture.Customizations.Add(new WebsiteSpecimenBuilder());
        this._fixture.Customizations.Add(new CompanyNameSpecimenBuilder());

        // 再加入類型層級的整合（優先級較低）
        // 使用種子感知的 SpecimenBuilder 以確保一致性
        this._fixture.Customizations.Add(new SeedAwareBogusSpecimenBuilder(this.GetCurrentSeed()));
    }

    /// <summary>
    /// 取得目前的 Seed
    /// </summary>
    /// <returns>目前的 Seed</returns>
    private int? GetCurrentSeed()
    {
        // 嘗試從 Randomizer 獲取當前種子
        return Randomizer.Seed?.Next();
    }
}