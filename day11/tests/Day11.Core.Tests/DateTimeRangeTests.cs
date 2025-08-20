using Day11.Core.Tests.TestHelpers;

namespace Day11.Core.Tests;

/// <summary>
/// 測試 DateTime 屬性值範圍控制
/// </summary>
public class DateTimeRangeTests
{
    [Fact]
    public void 使用RandomDateTimeSequenceGenerator_控制DateTime範圍()
    {
        // Arrange
        var fixture = new Fixture();
        var minDate = new DateTime(2025, 1, 1);
        var maxDate = new DateTime(2025, 12, 31);

        fixture.Customizations.Add(new RandomDateTimeSequenceGenerator(minDate, maxDate));

        // Act
        var member = fixture.Create<Member>();

        // Assert
        member.CreateTime.Should().BeOnOrAfter(minDate).And.BeOnOrBefore(maxDate);
        member.UpdateTime.Should().BeOnOrAfter(minDate).And.BeOnOrBefore(maxDate);
    }

    [Fact]
    public void RandomDateTimeSequenceGenerator_會影響所有DateTime屬性()
    {
        // Arrange
        var fixture = new Fixture();
        var minDate = new DateTime(2025, 1, 1);
        var maxDate = new DateTime(2025, 12, 31);

        fixture.Customizations.Add(new RandomDateTimeSequenceGenerator(minDate, maxDate));

        // Act
        var members = fixture.CreateMany<Member>(5).ToList();

        // Assert
        members.Should().AllSatisfy(member =>
        {
            // 所有的 DateTime 屬性都會被限制在同樣的範圍內
            member.CreateTime.Should().BeOnOrAfter(minDate).And.BeOnOrBefore(maxDate);
            member.UpdateTime.Should().BeOnOrAfter(minDate).And.BeOnOrBefore(maxDate);
        });
    }

    [Fact]
    public void 使用自訂RandomRangedDateTimeBuilder_只控制特定DateTime屬性()
    {
        // Arrange
        var fixture = new Fixture();

        var minDate = new DateTime(2025, 1, 1);
        var maxDate = new DateTime(2025, 12, 31);

        // 使用 RandomRangedDateTimeBuilder 控制 UpdateTime 屬性
        fixture.Customizations.Add(new RandomRangedDateTimeBuilder(minDate, maxDate, "UpdateTime"));

        var otherMinDate = new DateTime(2024, 1, 1);
        var otherMaxDate = new DateTime(2024, 12, 31);

        // 使用 RandomDateTimeSequenceGenerator 控制其他 DateTime 屬性
        fixture.Customizations.Add(new RandomDateTimeSequenceGenerator(otherMinDate, otherMaxDate));

        // Act
        var member = fixture.Create<Member>();

        // Assert
        // UpdateTime 應該在 2024-01-01 ~ 2024-12-31 之間
        member.UpdateTime.Should().BeOnOrAfter(minDate).And.BeOnOrBefore(maxDate);
        (member.UpdateTime >= minDate && member.UpdateTime <= maxDate).Should().BeTrue();
        (member.UpdateTime < otherMinDate || member.UpdateTime > otherMaxDate).Should().BeTrue();

        // CreateTime 應該在 2025-01-01 ~ 2025-12-31 之間
        member.CreateTime.Should().BeOnOrAfter(otherMinDate).And.BeOnOrBefore(otherMaxDate);
        (member.CreateTime >= otherMinDate && member.CreateTime <= otherMaxDate).Should().BeTrue();
        (member.CreateTime < minDate || member.CreateTime > maxDate).Should().BeTrue();
    }

    [Fact]
    public void RandomRangedDateTimeBuilder_應正確回傳NoSpecimen()
    {
        // Arrange
        var minDate = new DateTime(2025, 1, 1);
        var maxDate = new DateTime(2025, 12, 31);

        var builder = new RandomRangedDateTimeBuilder(minDate, maxDate, "CreateTime");

        var context = new SpecimenContext(new Fixture());

        // Act & Assert - 非目標屬性應回傳 NoSpecimen
        var stringPropertyRequest = typeof(Member).GetProperty("Name");
        var result1 = builder.Create(stringPropertyRequest, context);
        result1.Should().BeOfType<NoSpecimen>();

        // DateTime 但非目標屬性也應回傳 NoSpecimen
        var updateTimePropertyRequest = typeof(Member).GetProperty("UpdateTime");
        var result2 = builder.Create(updateTimePropertyRequest, context);
        result2.Should().BeOfType<NoSpecimen>();

        // 目標屬性應回傳 DateTime
        var createTimePropertyRequest = typeof(Member).GetProperty("CreateTime");
        var result3 = builder.Create(createTimePropertyRequest, context);
        result3.Should().BeOfType<DateTime>();
    }
}