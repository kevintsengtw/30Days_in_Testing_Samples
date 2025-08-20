namespace Day11.Core.Tests;

/// <summary>
/// 測試屬性值範圍控制
/// </summary>
public class PropertyRangeTests
{
    [Fact]
    public void 使用With方法_控制Member年齡範圍()
    {
        // Arrange
        var fixture = new Fixture();

        // Act
        var member = fixture.Build<Member>()
                            .With(x => x.Age, () => Random.Shared.Next(30, 50))
                            .Create();

        // Assert
        member.Age.Should().BeInRange(30, 50);
        member.Name.Should().NotBeNull();
        member.Email.Should().NotBeNull();
    }

    [Fact]
    public void CreateMany產生多個Member_每個Age都在指定範圍內()
    {
        // Arrange
        var fixture = new Fixture();

        // Act
        var members = fixture.Build<Member>()
                             .With(x => x.Age, () => Random.Shared.Next(30, 50))
                             .CreateMany(10)
                             .ToList();

        // Assert
        members.Should().HaveCount(10);
        members.Should().AllSatisfy(member => { member.Age.Should().BeInRange(30, 50); });

        // 驗證年齡確實是隨機的（不是所有物件都有相同年齡）
        var distinctAges = members.Select(m => m.Age).Distinct().Count();
        distinctAges.Should().BeGreaterThan(1);
    }
}