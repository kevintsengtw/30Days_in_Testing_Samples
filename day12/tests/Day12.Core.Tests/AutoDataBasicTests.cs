namespace Day12.Core.Tests;

/// <summary>
/// AutoData 基本使用方式測試
/// </summary>
public class AutoDataBasicTests
{
    [Theory]
    [AutoData]
    public void AutoData_應能自動產生所有參數(Person person, string message, int count)
    {
        // Arrange & Act - 參數已由 AutoData 自動產生

        // Assert
        person.Should().NotBeNull();
        person.Id.Should().NotBe(Guid.Empty);
        person.Name.Should().HaveLength(10);   // 遵循 StringLength 限制
        person.Age.Should().BeInRange(18, 80); // 遵循 Range 限制
        message.Should().NotBeNullOrEmpty();
        count.Should().NotBe(0);
    }

    [Theory]
    [AutoData]
    public void AutoData_透過DataAnnotation約束參數(
        [StringLength(5, MinimumLength = 3)] string shortName,
        [Range(1, 100)] int percentage,
        Person person)
    {
        // Arrange & Act - 已由 AutoData 根據 DataAnnotation 產生

        // Assert
        shortName.Length.Should().BeInRange(3, 5);
        percentage.Should().BeInRange(1, 100);
        person.Should().NotBeNull();
    }

    [Theory]
    [AutoData]
    public void AutoData_每次執行產生不同資料_第一次(Person person)
    {
        // Arrange & Act - 每次執行 person 都會有不同的值

        // Assert
        person.Should().NotBeNull();
        person.Age.Should().BeInRange(18, 80);

        // 每次執行都會有不同的 Name 值
        person.Name.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [AutoData]
    public void AutoData_每次執行產生不同資料_第二次(Person person)
    {
        // Arrange & Act - 每次執行 person 都會有不同的值

        // Assert
        person.Should().NotBeNull();
        person.Age.Should().BeInRange(18, 80);

        // 每次執行都會有不同的 Name 值
        person.Name.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [AutoData]
    public void AutoData_每次執行產生不同資料_第三次(Person person)
    {
        // Arrange & Act - 每次執行 person 都會有不同的值

        // Assert
        person.Should().NotBeNull();
        person.Age.Should().BeInRange(18, 80);

        // 每次執行都會有不同的 Name 值
        person.Name.Should().NotBeNullOrEmpty();
    }
}