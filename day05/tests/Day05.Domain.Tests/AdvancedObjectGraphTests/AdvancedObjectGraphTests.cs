using Day05.Domain.DomainModels;
using Day05.Domain.Services.BusinessServices;
using Day05.Domain.Services.ProcessingServices;

namespace Day05.Domain.Tests.AdvancedObjectGraphTests;

public class AdvancedObjectGraphTests
{
    private readonly TreeService _treeService;
    private readonly ComplexService _complexService;

    public AdvancedObjectGraphTests()
    {
        this._treeService = new TreeService();
        this._complexService = new ComplexService();
    }

    [Fact]
    public void ObjectGraph_循環引用處理_應正常運作()
    {
        // Arrange
        // 處理循環參考的物件比較
        var parent = new TreeNode { Value = "Root" };
        var child1 = new TreeNode { Value = "Child1", Parent = parent };
        var child2 = new TreeNode { Value = "Child2", Parent = parent };
        parent.Children = [child1, child2];

        // Act
        var actualTree = this._treeService.GetTree("Root");

        // Assert
        // 使用循環參考處理
        actualTree.Should().BeEquivalentTo(
            parent,
            options => options.IgnoringCyclicReferences().WithStrictOrdering()
        );
    }

    [Fact]
    public void ObjectGraph_效能最佳化比較_應正常運作()
    {
        // Arrange
        var largeObjectGraph = this.CreateLargeObjectGraph();

        // Act
        var actualGraph = this._complexService.ProcessEntity();

        // Assert
        // 效能最佳化的比較策略
        actualGraph.Should().BeEquivalentTo(
            largeObjectGraph,
            options => options.WithStrictOrdering()
                              .Including(x => x.ImportantProperty1) // 只比較關鍵屬性
                              .Including(x => x.ImportantProperty2)
                              .Including(x => x.CriticalData)
        );
    }

    private ComplexObject CreateLargeObjectGraph()
    {
        return new ComplexObject
        {
            ImportantProperty1 = "Value1",
            ImportantProperty2 = "Value2",
            CriticalData = "CriticalValue",
            Timestamp = DateTime.Now,
            GeneratedId = Guid.NewGuid()
        };
    }
}