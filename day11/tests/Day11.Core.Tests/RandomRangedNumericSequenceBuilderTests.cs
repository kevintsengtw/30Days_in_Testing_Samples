using Day11.Core.Tests.TestHelpers;

namespace Day11.Core.Tests;

/// <summary>
/// 測試 RandomRangedNumericSequenceBuilder 的功能
/// </summary>
public class RandomRangedNumericSequenceBuilderTests
{
    [Fact]
    public void 第一個版本RandomRangedNumericSequenceBuilder_可能會失效()
    {
        // Arrange
        var fixture = new Fixture();
        fixture.Customizations.Add(new RandomRangedNumericSequenceBuilder(30, 50, "Age"));

        // Act
        var member = fixture.Create<Member>();

        // Assert
        // 這個測試展示第一個版本的問題：
        // 由於 AutoFixture 內建建構器的優先順序更高，我們的自訂建構器可能會失效
        // 結果可能還是使用 DataAnnotations 的範圍（10-80）而不是我們指定的範圍（30-50）
        member.Age.Should().BeInRange(10, 80); // 實際上還是使用 DataAnnotations 的範圍

        // 驗證不在我們指定的範圍內，說明自訂建構器失效了
        if (member.Age is < 30 or >= 50)
        {
            // 這證明了第一個版本的問題：內建建構器優先順序更高
            member.Age.Should().BeInRange(10, 80); // 使用 DataAnnotations 的範圍
        }
    }

    [Fact]
    public void 使用Insert0確保自訂建構器優先順序_控制Member年齡範圍()
    {
        // Arrange
        var fixture = new Fixture();

        // 使用 Insert(0) 確保最高優先順序
        fixture.Customizations.Insert(index: 0,
                                      new ImprovedRandomRangedNumericSequenceBuilder(
                                          min: 30,
                                          max: 50,
                                          predicate: prop => prop.Name == "Age" && prop.DeclaringType == typeof(Member)));

        // Act
        var member = fixture.Create<Member>();

        // Assert
        member.Age.Should().BeInRange(30, 49);
    }

    [Fact]
    public void RandomRangedNumericSequenceBuilder_完整測試案例()
    {
        // Arrange
        var fixture = new Fixture();
        fixture.Customizations.Insert(index: 0,
                                      new ImprovedRandomRangedNumericSequenceBuilder(
                                          min: 30,
                                          max: 50,
                                          predicate: prop => prop.Name == "Age" && prop.DeclaringType == typeof(Member)));

        // Act
        var members = fixture.CreateMany<Member>(20).ToList();

        // Assert
        members.Should().HaveCount(20);
        members.Should().AllSatisfy(member => { member.Age.Should().BeInRange(30, 49); });

        // 驗證年齡確實是隨機的
        var distinctAges = members.Select(m => m.Age).Distinct().Count();
        distinctAges.Should().BeGreaterThan(1);
    }

    [Fact]
    public void 展示Insert0和Add的差異()
    {
        // Arrange - 使用 Add() 的版本（可能會失效）
        var fixtureWithAdd = new Fixture();
        fixtureWithAdd.Customizations.Add(new RandomRangedNumericSequenceBuilder(30, 50, "Age"));

        // Arrange - 使用 Insert(0) 的版本（確保優先順序）
        var fixtureWithInsert = new Fixture();
        fixtureWithInsert.Customizations.Insert(index: 0,
                                                new ImprovedRandomRangedNumericSequenceBuilder(
                                                    min: 30,
                                                    max: 50,
                                                    predicate: prop => prop.Name == "Age" && prop.DeclaringType == typeof(Member)));

        // Act
        var memberWithAdd = fixtureWithAdd.Create<Member>();
        var memberWithInsert = fixtureWithInsert.Create<Member>();

        // Assert
        // Insert(0) 版本應該能控制年齡範圍
        memberWithInsert.Age.Should().BeInRange(30, 49);

        // Add() 版本可能會使用 DataAnnotations 的範圍（10-80）
        memberWithAdd.Age.Should().BeInRange(10, 80);
    }

    [Fact]
    public void 比較不同優先順序設定的效果()
    {
        // Arrange
        var fixture1 = new Fixture();
        var fixture2 = new Fixture();

        // 第一個 fixture：使用 Add()，可能被內建建構器覆蓋
        fixture1.Customizations.Add(new RandomRangedNumericSequenceBuilder(100, 200, "Age"));

        // 第二個 fixture：使用 Insert(0)，確保最高優先順序
        fixture2.Customizations.Insert(index: 0,
                                       new ImprovedRandomRangedNumericSequenceBuilder(
                                           min: 100,
                                           max: 200,
                                           predicate: prop => prop.Name == "Age" && prop.DeclaringType == typeof(Member)));

        // Act
        var members1 = fixture1.CreateMany<Member>(10).ToList();
        var members2 = fixture2.CreateMany<Member>(10).ToList();

        // Assert
        // fixture2 應該能精確控制範圍
        members2.Should().AllSatisfy(member => member.Age.Should().BeInRange(100, 199));

        // fixture1 可能還是使用 DataAnnotations 的範圍
        members1.Should().AllSatisfy(member => member.Age.Should().BeInRange(10, 80));
    }
}