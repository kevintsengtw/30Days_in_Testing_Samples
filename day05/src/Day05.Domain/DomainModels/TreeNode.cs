namespace Day05.Domain.DomainModels;

/// <summary>
/// class TreeNode - 樹狀結構節點
/// </summary>
public class TreeNode
{
    /// <summary>
    /// 節點值
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// 父節點
    /// </summary>
    public TreeNode? Parent { get; set; }

    /// <summary>
    /// 子節點
    /// </summary>
    public TreeNode[] Children { get; set; } = [];
}