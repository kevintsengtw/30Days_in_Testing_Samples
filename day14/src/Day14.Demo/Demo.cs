using System.Diagnostics;
using Bogus;
using Day14.Core.Generators;
using Day14.Core.Models;

namespace Day14.Demo;

public class Demo
{
    /// <summary>
    /// 基本 Bogus 功能示範
    /// </summary>
    internal static void DemonstrateBogusBasics()
    {
        Console.WriteLine("1. 基本 Bogus 功能示範");
        Console.WriteLine("─────────────────────");

        // 產生產品資料
        var products = BogusDataGenerator.GenerateProducts(3);
        Console.WriteLine("產生的產品資料：");
        foreach (var product in products)
        {
            Console.WriteLine($"  - {product.Name} (${product.Price:F2}) - {product.Category}");
        }

        // 產生使用者資料
        var users = BogusDataGenerator.GenerateUsers(3);
        Console.WriteLine("\n產生的使用者資料：");
        foreach (var user in users)
        {
            Console.WriteLine($"  - {user.FullName} ({user.Email}) - {user.Department} {user.Role}");
        }

        Console.WriteLine();
    }

    /// <summary>
    /// 台灣本土化資料示範
    /// </summary>
    internal static void DemonstrateTaiwanData()
    {
        Console.WriteLine("2. 台灣本土化資料示範");
        Console.WriteLine("─────────────────────");

        var taiwanPeople = BogusDataGenerator.GenerateTaiwanPeople(5);
        Console.WriteLine("產生的台灣人員資料：");
        foreach (var person in taiwanPeople)
        {
            Console.WriteLine($"  - {person.Name}");
            Console.WriteLine($"    城市: {person.City}");
            Console.WriteLine($"    大學: {person.University}");
            Console.WriteLine($"    公司: {person.Company}");
            Console.WriteLine($"    手機: {person.Mobile}");
            Console.WriteLine($"    身分證: {person.IdCard}");
            Console.WriteLine();
        }
    }

    /// <summary>
    /// 複雜物件關係示範
    /// </summary>
    internal static void DemonstrateComplexObjects()
    {
        Console.WriteLine("3. 複雜物件關係示範");
        Console.WriteLine("─────────────────────");

        // 產生員工資料（包含專案經驗）
        var employees = BogusDataGenerator.GenerateEmployees(2);
        Console.WriteLine("產生的員工資料：");
        foreach (var employee in employees)
        {
            Console.WriteLine($"  - {employee.FirstName} {employee.LastName} ({employee.EmployeeId})");
            Console.WriteLine($"    職級: {employee.Level}, 年齡: {employee.Age}, 薪資: ${employee.Salary:F0}");
            Console.WriteLine($"    技能: {string.Join(", ", employee.Skills)}");
            Console.WriteLine($"    專案經驗 ({employee.Projects.Count} 個):");
            foreach (var project in employee.Projects.Take(2))
            {
                Console.WriteLine($"      * {project.Name} ({project.StartDate:yyyy/MM} - {project.EndDate:yyyy/MM})");
                Console.WriteLine($"        技術: {string.Join(", ", project.Technologies)}");
            }

            Console.WriteLine();
        }

        // 產生訂單資料
        var orders = BogusDataGenerator.GenerateOrders(2);
        Console.WriteLine("產生的訂單資料：");
        foreach (var order in orders)
        {
            Console.WriteLine($"  - 訂單號碼: {order.OrderNumber}");
            Console.WriteLine($"    客戶: {order.CustomerName} ({order.CustomerEmail})");
            Console.WriteLine($"    狀態: {order.Status}, 日期: {order.OrderDate:yyyy/MM/dd}");
            Console.WriteLine($"    項目數量: {order.Items.Count}, 總金額: ${order.TotalAmount:F2}");
            Console.WriteLine();
        }
    }

    /// <summary>
    /// AutoBogus 功能示範
    /// </summary>
    internal static void DemonstrateAutoBogus()
    {
        Console.WriteLine("4. AutoBogus 功能示範");
        Console.WriteLine("─────────────────────");

        // 基本 AutoBogus 使用
        var autoFixtureUser = AutoBogusDataGenerator.CreateUserWithAutoFixture();
        Console.WriteLine("AutoBogus 產生的使用者：");
        Console.WriteLine($"  - {autoFixtureUser.FirstName} {autoFixtureUser.LastName} ({autoFixtureUser.Email})");
        Console.WriteLine($"    年齡: {autoFixtureUser.Age}, 積分: {autoFixtureUser.Points}");

        // 自訂 AutoBogus 設定
        var customUser = AutoBogusDataGenerator.CreateCustomUser("test@example.com", 25);
        Console.WriteLine("\n自訂 AutoFixture 產生的使用者：");
        Console.WriteLine($"  - {customUser.FirstName} {customUser.LastName} ({customUser.Email}) - 年齡: {customUser.Age}");

        Console.WriteLine();
    }

    /// <summary>
    /// 效能比較示範
    /// </summary>
    internal static void DemonstratePerformanceComparison()
    {
        Console.WriteLine("5. 效能比較示範");
        Console.WriteLine("─────────────────────");

        const int testCount = 1000;

        // Bogus 效能測試
        var stopwatch = Stopwatch.StartNew();
        var bogusUsers = BogusDataGenerator.GenerateUsers(testCount);
        stopwatch.Stop();
        Console.WriteLine($"Bogus 產生 {testCount} 個使用者耗時: {stopwatch.ElapsedMilliseconds} ms");

        // AutoBogus 效能測試
        stopwatch.Restart();
        var autoBogusUsers = AutoBogusDataGenerator.CreateMultipleUsers(testCount);
        stopwatch.Stop();
        Console.WriteLine($"AutoBogus 產生 {testCount} 個使用者耗時: {stopwatch.ElapsedMilliseconds} ms");

        Console.WriteLine($"記憶體使用差異: Bogus={GC.GetTotalMemory(false):N0} bytes");
        Console.WriteLine();
    }

    /// <summary>
    /// 可重現性示範
    /// </summary>
    internal static void DemonstrateReproducibility()
    {
        Console.WriteLine("6. 可重現性示範");
        Console.WriteLine("─────────────────────");

        const int seed = 12345;

        // 第一次產生（使用 seed）
        var faker1 = new Faker<Product>().UseSeed(seed)
                                         .RuleFor(p => p.Id, f => f.IndexFaker)
                                         .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                                         .RuleFor(p => p.Price, f => f.Random.Decimal(10, 1000));

        var products1 = faker1.Generate(3);
        Console.WriteLine("第一次產生的產品名稱：");
        foreach (var product in products1)
        {
            Console.WriteLine($"  - {product.Name}");
        }

        // 第二次產生（相同 seed）
        var faker2 = new Faker<Product>().UseSeed(seed)
                                         .RuleFor(p => p.Id, f => f.IndexFaker)
                                         .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                                         .RuleFor(p => p.Price, f => f.Random.Decimal(10, 1000));

        var products2 = faker2.Generate(3);
        Console.WriteLine("\n第二次產生的產品名稱（相同 seed）：");
        foreach (var product in products2)
        {
            Console.WriteLine($"  - {product.Name}");
        }

        var isIdentical = products1.Zip(products2, (p1, p2) => p1.Name == p2.Name).All(x => x);
        Console.WriteLine($"\n兩次產生結果是否相同: {isIdentical}");
        Console.WriteLine();
    }
}