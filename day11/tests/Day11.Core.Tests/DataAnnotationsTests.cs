namespace AutoFixtureAdvanced.Tests;

/// <summary>
/// 測試 DataAnnotations 整合應用
/// </summary>
public class DataAnnotationsTests
{
    [Fact]
    public void AutoFixture_應能識別DataAnnotations並產生符合限制的資料()
    {
        // Arrange
        var fixture = new Fixture();

        // Act
        var person = fixture.Create<Person>();

        // Assert
        person.Id.Should().NotBe(Guid.Empty);
        person.Name.Should().NotBeNull();
        person.Name.Length.Should().Be(10);    // StringLength(10) 的限制
        person.Age.Should().BeInRange(10, 80); // Range(10, 80) 的限制
        person.CreateTime.Should().BeAfter(DateTime.MinValue);
    }

    [Fact]
    public void AutoFixture_批量產生的Person物件_都應符合DataAnnotations限制()
    {
        // Arrange
        var fixture = new Fixture();

        // Act
        var persons = fixture.CreateMany<Person>(10).ToList();

        // Assert
        persons.Should().HaveCount(10);
        persons.Should().AllSatisfy(person =>
        {
            person.Name.Length.Should().Be(10);
            person.Age.Should().BeInRange(10, 80);
        });
    }
}