using Day05.Domain.DomainModels;

namespace Day05.Domain.Services.BusinessServices;

/// <summary>
/// class TreeService - 樹狀結構服務
/// </summary>
public class TreeService
{
    /// <summary>
    /// 獲取樹狀結構
    /// </summary>
    /// <param name="rootValue">根節點值</param>
    /// <returns>樹狀結構根節點</returns>
    public TreeNode GetTree(string rootValue)
    {
        var parent = new TreeNode { Value = rootValue };
        var child1 = new TreeNode { Value = "Child1", Parent = parent };
        var child2 = new TreeNode { Value = "Child2", Parent = parent };
        parent.Children = [child1, child2];

        return parent;
    }
}