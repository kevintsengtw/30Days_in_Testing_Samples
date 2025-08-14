using Day05.Domain.Exceptions;

namespace Day05.Domain.Services.ProcessingServices;

/// <summary>
/// class BatchProcessingService - 批量處理服務
/// </summary>
public class BatchProcessingService
{
    /// <summary>
    /// 處理批量任務
    /// </summary>
    public void ProcessBatch(object[] items)
    {
        var exceptions = new List<ValidationException>();

        foreach (var item in items)
        {
            try
            {
                if (item is null)
                {
                    throw new ValidationException("項目不可為空值");
                }
            }
            catch (ValidationException ex)
            {
                exceptions.Add(ex);
            }
        }

        if (exceptions.Count != 0)
        {
            throw new AggregateException(exceptions);
        }
    }
}