using Day05.Domain.Exceptions;
using Day05.Domain.Services.ProcessingServices;

namespace Day05.Domain.Tests.AdvancedExceptionAssertionTests;

public class AdvancedExceptionAssertionTests
{
    [Fact]
    public void Exception_巢狀例外_應拋出例外DatabaseConnectionException與ArgumentException()
    {
        // Arrange
        var databaseService = new DatabaseService();

        // Act
        // 巢狀例外 Assertions
        var action = () => databaseService.Connect("invalid-connection-string");

        // Assert
        action.Should().NotBeNull(); // 確保 action 不為 null
        action.Should().Throw<DatabaseConnectionException>()
              .WithInnerException<ArgumentException>()
              .WithMessage("*連線字串不可為無效值*");
    }

    [Fact]
    public void Exception_聚合例外_應拋出例外AggregateException與ValidationException()
    {
        // Arrange
        var batchService = new BatchProcessingService();
        var invalidItems = new object[] { null!, null!, null! };

        // Act
        // 聚合例外 Assertions
        var action = () => batchService.ProcessBatch(invalidItems);

        // Assert
        action.Should().Throw<AggregateException>()
              .Which.InnerExceptions.Should().HaveCount(3)
              .And.AllBeOfType<ValidationException>();
    }
}