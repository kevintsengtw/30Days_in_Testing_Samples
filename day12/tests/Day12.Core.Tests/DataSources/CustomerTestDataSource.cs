using System.Text.Json;

namespace Day12.Core.Tests.DataSources;

/// <summary>
/// 客戶測試資料來源
/// </summary>
public class CustomerTestDataSource : BaseTestData
{
    public static IEnumerable<object[]> VipCustomers()
    {
        var testDataPath = Path.Combine(AppContext.BaseDirectory, "TestData", "customers.json");
        if (!File.Exists(testDataPath))
        {
            // 如果在輸出目錄找不到，嘗試從專案目錄找
            testDataPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "TestData", "customers.json");
        }

        if (!File.Exists(testDataPath))
        {
            // 如果還是找不到，返回預設資料
            yield return new object[] { 1001, "張三", "zhang.san@example.com", "VIP", 50000m };
            yield break;
        }

        var jsonContent = File.ReadAllText(testDataPath);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var customers = JsonSerializer.Deserialize<List<CustomerJsonRecord>>(jsonContent, options);

        foreach (var customer in customers ?? new List<CustomerJsonRecord>())
        {
            if (customer.Level == "VIP")
            {
                yield return
                [
                    customer.CustomerId,
                    customer.Name,
                    customer.Email,
                    customer.Level,
                    customer.CreditLimit
                ];
            }
        }
    }

    public static IEnumerable<object[]> RegularCustomers()
    {
        var testDataPath = Path.Combine(AppContext.BaseDirectory, "TestData", "customers.json");
        if (!File.Exists(testDataPath))
        {
            // 如果在輸出目錄找不到，嘗試從專案目錄找
            testDataPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "TestData", "customers.json");
        }

        if (!File.Exists(testDataPath))
        {
            // 如果還是找不到，返回預設資料
            yield return [1003, "王五", "wang.wu@example.com", "Regular", 10000m];
            yield break;
        }

        var jsonContent = File.ReadAllText(testDataPath);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var customers = JsonSerializer.Deserialize<List<CustomerJsonRecord>>(jsonContent, options);

        foreach (var customer in customers ?? new List<CustomerJsonRecord>())
        {
            if (customer.Level == "Regular")
            {
                yield return
                [
                    customer.CustomerId,
                    customer.Name,
                    customer.Email,
                    customer.Level,
                    customer.CreditLimit
                ];
            }
        }
    }
}