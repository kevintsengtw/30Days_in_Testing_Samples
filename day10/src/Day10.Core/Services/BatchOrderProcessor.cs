using Day10.Core.Models;

namespace Day10.Core.Services;

/// <summary>
/// 批次訂單處理器
/// </summary>
public class BatchOrderProcessor
{
    /// <summary>
    /// 批次處理訂單
    /// </summary>
    public BatchProcessResult ProcessBatch(IEnumerable<Order> orders)
    {
        var processor = new OrderProcessor();
        var results = new List<ProcessResult>();

        foreach (var order in orders)
        {
            results.Add(processor.Process(order));
        }

        return new BatchProcessResult
        {
            SuccessCount = results.Count(r => r.Success),
            FailureCount = results.Count(r => !r.Success),
            Results = results
        };
    }
}