using AutoFixture;
using Day15.TestLibrary.TestData.Extensions;
using Day15.TestLibrary.TestData.Factories;

namespace Day15.TestLibrary.TestData.Base;

/// <summary>
/// 測試基底類別，提供統一的資料產生功能
/// </summary>
public abstract class TestBase
{
    /// <summary>
    /// AutoFixture 實例，用於產生測試資料
    /// </summary>
    protected readonly IFixture Fixture;

    /// <summary>
    /// 混合測試資料產生器，結合多種資料產生策略
    /// </summary>
    protected readonly HybridTestDataGenerator Generator;

    /// <summary>
    /// 整合測試資料工廠，提供更高階的物件建立功能
    /// </summary>
    protected readonly IntegratedTestDataFactory Factory;

    /// <summary>
    /// 建構式，初始化 AutoFixture、混合產生器及整合工廠
    /// </summary>
    /// <param name="seed"></param>
    protected TestBase(int? seed = null)
    {
        // 建立統一設定的 AutoFixture
        this.Fixture = new Fixture()
                       .WithBogus()
                       .WithOmitOnRecursion()
                       .WithRepeatCount(3);

        if (seed.HasValue)
        {
            this.Fixture.WithSeed(seed.Value);
        }

        // 建立混合產生器
        this.Generator = new HybridTestDataGenerator(seed);

        // 建立整合工廠
        this.Factory = new IntegratedTestDataFactory(seed);
    }

    /// <summary>
    /// 快速建立單一物件
    /// </summary>
    protected T Create<T>()
    {
        return this.Fixture.Create<T>();
    }

    /// <summary>
    /// 快速建立多個物件
    /// </summary>
    protected List<T> CreateMany<T>(int count = 3)
    {
        return this.Fixture.CreateMany<T>(count).ToList();
    }

    /// <summary>
    /// 建立並設定物件
    /// </summary>
    protected T Create<T>(Action<T> configure)
    {
        var instance = this.Create<T>();
        configure(instance);
        return instance;
    }

    /// <summary>
    /// 使用混合產生器建立物件
    /// </summary>
    protected T Generate<T>()
    {
        return this.Generator.Generate<T>();
    }

    /// <summary>
    /// 使用工廠建立物件
    /// </summary>
    protected T FactoryCreate<T>() where T : class
    {
        return this.Factory.CreateFresh<T>();
    }

    /// <summary>
    /// 清理方法，在測試結束時呼叫
    /// </summary>
    protected virtual void Cleanup()
    {
        this.Factory.ClearCache();
    }
}