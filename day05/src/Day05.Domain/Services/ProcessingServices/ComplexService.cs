using Day05.Domain.DomainModels;

namespace Day05.Domain.Services.ProcessingServices;

/// <summary>
/// class ComplexService - 複雜服務
/// </summary>
public class ComplexService
{
    /// <summary>
    /// 處理複雜實體
    /// </summary>
    /// <returns>處理後的複雜物件</returns>
    public ComplexObject ProcessEntity()
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

    /// <summary>
    /// 生成複雜物件
    /// </summary>
    /// <returns>新的複雜物件</returns>
    public ComplexObject GenerateComplexObject()
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