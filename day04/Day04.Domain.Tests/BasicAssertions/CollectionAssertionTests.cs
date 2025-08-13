using Day04.Domain.Models;

namespace Day04.Domain.Tests.BasicAssertions;

/// <summary>
/// class CollectionAssertionTests - 集合斷言測試
/// </summary>
public class CollectionAssertionTests
{
    [Fact]
    public void HaveCount_Contain_基本集合檢查_應驗證數量和內容()
    {
        var numbers = new[] { 1, 2, 3, 4, 5 };
        var emptyList = new List<int>();

        // 基本集合斷言
        numbers.Should().NotBeEmpty()
               .And.HaveCount(5)
               .And.Contain(3)
               .And.NotContain(10);

        emptyList.Should().BeEmpty()
                 .And.HaveCount(0);
    }

    [Fact]
    public void BeInAscendingOrder_OnlyHaveUniqueItems_排序和唯一性_應符合要求()
    {
        var sortedNumbers = new[] { 1, 2, 3, 4, 5 };
        var unsortedNumbers = new[] { 3, 1, 4, 2, 5 };
        var duplicateNumbers = new[] { 1, 2, 2, 3, 3, 3 };

        // 順序斷言
        sortedNumbers.Should().BeInAscendingOrder();
        unsortedNumbers.Should().NotBeInAscendingOrder();

        // 唯一性斷言
        sortedNumbers.Should().OnlyHaveUniqueItems();
        duplicateNumbers.Should().Contain(x => duplicateNumbers.Count(y => y == x) > 1, "因為陣列包含重複項目");
    }

    [Fact]
    public void AllSatisfy_複雜物件集合_應滿足所有條件()
    {
        var users = new[]
        {
            new User { Id = 1, Name = "Alice", Age = 25 },
            new User { Id = 2, Name = "Bob", Age = 30 },
            new User { Id = 3, Name = "Charlie", Age = 35 }
        };

        // 複雜物件集合斷言
        users.Should().HaveCount(3)
             .And.Contain(u => u.Name == "Alice")
             .And.AllSatisfy(u => u.Age.Should().BeGreaterThan(20));

        // 集合屬性驗證
        users.Select(u => u.Name).Should().Contain("Bob")
             .And.NotContain("David");

        users.Where(u => u.Age > 30).Should().HaveCount(1);
    }
}