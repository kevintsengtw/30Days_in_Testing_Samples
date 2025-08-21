using System.Reflection;
using AutoFixture;

namespace Day12.Core.Tests.Attributes;

/// <summary>
/// 複合自訂 AutoData 屬性，允許組合多個 AutoData 配置
/// </summary>
public class CompositeAutoDataAttribute : AutoDataAttribute
{
    public CompositeAutoDataAttribute(params Type[] autoDataAttributeTypes) : base(() => CreateFixture(autoDataAttributeTypes))
    {
    }

    private static IFixture CreateFixture(Type[] autoDataAttributeTypes)
    {
        var fixture = new Fixture();

        foreach (var attributeType in autoDataAttributeTypes)
        {
            // 取得每個 AutoData 屬性類型的 CreateFixture 方法
            var createFixtureMethod = attributeType.GetMethod("CreateFixture", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            if (createFixtureMethod is not null)
            {
                // 執行 CreateFixture 方法並取得設定
                var sourceFixture = (IFixture)createFixtureMethod.Invoke(null, null)!;

                // 將來源 Fixture 的自訂設定複製到目標 Fixture
                foreach (var customization in sourceFixture.Customizations)
                {
                    fixture.Customizations.Add(customization);
                }
            }
        }

        return fixture;
    }
}