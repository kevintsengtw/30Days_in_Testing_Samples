namespace Day11.Core.Tests;

/// <summary>
/// 測試 .With() 方法的行為差異
/// </summary>
public class WithBehaviorTests
{
    [Fact]
    public void With方法_固定值vs動態值的差異()
    {
        // Arrange
        var fixture = new Fixture();

        // Act - 使用固定值（不用 lambda，只會執行一次）
        var membersWithFixedAge = fixture.Build<Member>()
                                         .With(x => x.Age, Random.Shared.Next(30, 50)) // 固定值，只執行一次隨機產生，所有物件都是同樣的值
                                         .CreateMany(5)
                                         .ToList();

        // Act - 使用動態值（用 lambda，每個物件都會執行一次）
        var membersWithDynamicAge = fixture.Build<Member>()
                                           .With(x => x.Age, () => Random.Shared.Next(30, 50)) // 動態值，每個物件都重新計算
                                           .CreateMany(5)
                                           .ToList();

        // Assert
        // 固定值：所有物件的年齡都相同（雖然是隨機產生的，但只產生一次）
        var firstAge = membersWithFixedAge.First().Age;
        firstAge.Should().BeInRange(30, 49);
        membersWithFixedAge.Should().AllSatisfy(member => member.Age.Should().Be(firstAge));
        var distinctFixedAges = membersWithFixedAge.Select(m => m.Age).Distinct().Count();
        distinctFixedAges.Should().Be(1); // 只有一個不同的值

        // 動態值：每個物件的年齡都可能不同
        membersWithDynamicAge.Should().AllSatisfy(member => member.Age.Should().BeInRange(30, 49));
        var distinctDynamicAges = membersWithDynamicAge.Select(m => m.Age).Distinct().Count();
        distinctDynamicAges.Should().BeGreaterThan(1); // 通常會有多個不同的值
    }

    [Fact]
    public void 展示Random共用實例的優勢()
    {
        // Arrange
        var fixture = new Fixture();

        // Act - 使用 Random.Shared（推薦）
        var members1 = fixture.Build<Member>()
                              .With(x => x.Age, () => Random.Shared.Next(20, 60))
                              .CreateMany(20)
                              .ToList();

        // Assert
        members1.Should().AllSatisfy(member => member.Age.Should().BeInRange(20, 60));

        // 驗證產生的年齡有足夠的隨機性
        var distinctAges = members1.Select(m => m.Age).Distinct().Count();
        distinctAges.Should().BeGreaterThan(5); // 在 20 個物件中，應該有超過 5 個不同的年齡值
    }
}