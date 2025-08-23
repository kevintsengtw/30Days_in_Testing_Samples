using Day14.Core.Extensions;
using Day14.Core.Models;

namespace Day14.Core.Generators;

/// <summary>
/// Bogus 資料產生器
/// </summary>
public static class BogusDataGenerator
{
    /// <summary>
    /// 產品 Faker
    /// </summary>
    public static readonly Faker<Product> ProductFaker =
        new Faker<Product>().RuleFor(p => p.Id, f => f.IndexFaker)
                            .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                            .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
                            .RuleFor(p => p.Price, f => f.Random.Decimal(10, 1000))
                            .RuleFor(p => p.Category, f => f.Commerce.Categories(1)[0])
                            .RuleFor(p => p.CreatedDate, f => f.Date.Past())
                            .RuleFor(p => p.StockQuantity, f => f.Random.Int(0, 500))
                            .RuleFor(p => p.IsActive, f => f.Random.Bool(0.9f))
                            .RuleFor(p => p.Tags, f => f.PickRandom(
                                         ["New", "Sale", "Featured", "Limited", "Premium", "Eco-Friendly"],
                                         f.Random.Int(0, 3)).ToList());

    /// <summary>
    /// 使用者 Faker
    /// </summary>
    public static readonly Faker<User> UserFaker =
        new Faker<User>().RuleFor(u => u.Id, f => f.Random.Guid())
                         .RuleFor(u => u.FirstName, f => f.Person.FirstName)
                         .RuleFor(u => u.LastName, f => f.Person.LastName)
                         .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
                         .RuleFor(u => u.Phone, f => f.Phone.PhoneNumber().OrNull(f, 0.2f))
                         .RuleFor(u => u.Age, f => f.Random.Int(18, 80))
                         .RuleFor(u => u.IsPremium, f => f.Random.Bool(0.8f))
                         .RuleFor(u => u.Points, (f, u) => u.IsPremium ? f.Random.Int(1000, 5000) : f.Random.Int(0, 500))
                         .RuleFor(u => u.Nickname, f => f.Person.FirstName.OrDefault(f, 0.3f, ""))
                         .RuleFor(u => u.Department, f => f.PickRandom("IT", "HR", "Finance", "Marketing"))
                         .RuleFor(u => u.Role, f => f.PickRandom("User", "Admin", "SuperAdmin"))
                         .RuleFor(u => u.CreatedAt, f => f.Date.Past(2));

    /// <summary>
    /// 台灣人 Faker
    /// </summary>
    public static readonly Faker<TaiwanPerson> TaiwanPersonFaker =
        new Faker<TaiwanPerson>().RuleFor(p => p.Id, f => f.Random.Guid())
                                 .RuleFor(p => p.Name, f => f.Person.FullName)
                                 .RuleFor(p => p.City, f => f.PickRandom(TaiwanDataExtensions.Cities))
                                 .RuleFor(p => p.University, f => f.PickRandom(TaiwanDataExtensions.Universities))
                                 .RuleFor(p => p.Company, f => f.PickRandom(TaiwanDataExtensions.Companies))
                                 .RuleFor(p => p.IdCard, f => f.Random.Replace("?#########"))
                                 .RuleFor(p => p.Mobile, f => "09" + f.Random.Replace("########"));

    /// <summary>
    /// 訂單項目 Faker
    /// </summary>
    public static readonly Faker<OrderItem> OrderItemFaker =
        new Faker<OrderItem>().RuleFor(i => i.Id, f => f.Random.Guid())
                              .RuleFor(i => i.ProductName, f => f.Commerce.ProductName())
                              .RuleFor(i => i.UnitPrice, f => f.Random.Decimal(10, 1000))
                              .RuleFor(i => i.Quantity, f => f.Random.Int(1, 10));

    /// <summary>
    /// 訂單 Faker
    /// </summary>
    public static readonly Faker<Order> OrderFaker =
        new Faker<Order>().RuleFor(o => o.Id, f => f.Random.Guid())
                          .RuleFor(o => o.OrderNumber, f => "ORD-" + f.Random.AlphaNumeric(8).ToUpper())
                          .RuleFor(o => o.CustomerName, f => f.Person.FullName)
                          .RuleFor(o => o.CustomerEmail, f => f.Person.Email)
                          .RuleFor(o => o.OrderDate, f => f.Date.Recent(30))
                          .RuleFor(o => o.Items, f => OrderItemFaker.Generate(f.Random.Int(1, 5)))
                          .RuleFor(o => o.SubTotal, (f, o) => o.Items.Sum(i => i.TotalPrice))
                          .RuleFor(o => o.TaxAmount, (f, o) => Math.Round(o.SubTotal * 0.05m, 2))
                          .RuleFor(o => o.ShippingFee, f => f.Random.Decimal(0, 50))
                          .RuleFor(o => o.TotalAmount, (f, o) => o.SubTotal + o.TaxAmount + o.ShippingFee)
                          .RuleFor(o => o.Status, f => f.PickRandom<OrderStatus>());

    /// <summary>
    /// 專案 Faker
    /// </summary>
    public static readonly Faker<Project> ProjectFaker =
        new Faker<Project>().RuleFor(p => p.Id, f => f.Random.Guid())
                            .RuleFor(p => p.Name, f => f.Commerce.ProductName() + " " + f.Hacker.Abbreviation())
                            .RuleFor(p => p.Description, f => f.Lorem.Sentence(10))
                            .RuleFor(p => p.StartDate, f => f.Date.Past())
                            .RuleFor(p => p.EndDate, (f, p) => f.Date.Between(p.StartDate, p.StartDate.AddMonths(6)))
                            .RuleFor(p => p.Technologies, f => f.PickRandom(
                                         [
                                             "C#", "JavaScript", "Python", "Java", "React", "Angular", "Vue", "Node.js",
                                             "Docker", "Kubernetes", "AWS", "Azure", "SQL Server", "MongoDB"
                                         ],
                                         f.Random.Int(2, 6)).ToList());

    /// <summary>
    /// 員工 Faker
    /// </summary>
    public static readonly Faker<Employee> EmployeeFaker =
        new Faker<Employee>().RuleFor(e => e.Id, f => f.Random.Guid())
                             .RuleFor(e => e.EmployeeId, f => "EMP-" + f.Random.AlphaNumeric(6).ToUpper())
                             .RuleFor(e => e.FirstName, f => f.Person.FirstName)
                             .RuleFor(e => e.LastName, f => f.Person.LastName)
                             .RuleFor(e => e.Email, (f, e) => $"{e.FirstName.ToLower()}.{e.LastName.ToLower()}@company.com")
                             .RuleFor(e => e.Department, f => f.PickRandom("Engineering", "Marketing", "Sales", "HR", "Finance"))
                             .RuleFor(e => e.Level, f => f.PickRandom("Junior", "Senior", "Lead", "Principal"))
                             .RuleFor(e => e.Age, (f, e) => e.Level switch
                             {
                                 "Junior" => f.Random.Int(22, 24),
                                 "Senior" => f.Random.Int(25, 34),
                                 "Lead" => f.Random.Int(35, 44),
                                 "Principal" => f.Random.Int(45, 65),
                                 _ => f.Random.Int(22, 65)
                             })
                             .RuleFor(e => e.Salary, (f, e) => e.Level switch
                             {
                                 "Junior" => f.Random.Decimal(35000, 50000),
                                 "Senior" => f.Random.Decimal(50000, 80000),
                                 "Lead" => f.Random.Decimal(80000, 120000),
                                 "Principal" => f.Random.Decimal(120000, 200000),
                                 _ => f.Random.Decimal(35000, 200000)
                             })
                             .RuleFor(e => e.HireDate, f => f.Date.Past(10))
                             .RuleFor(e => e.Skills, f => f.PickRandom(
                                          [
                                              "C#", "JavaScript", "Python", "Java", "React", "Angular", "Vue", "Node.js",
                                              "Docker", "Kubernetes", "AWS", "Azure", "SQL Server", "MongoDB", "Git", "Agile"
                                          ],
                                          f.Random.Int(3, 8)).ToList())
                             .RuleFor(e => e.Projects, (f, e) =>
                             {
                                 var projects = ProjectFaker.Generate(f.Random.Int(1, 4));
                                 foreach (var project in projects)
                                 {
                                     project.StartDate = f.Date.Between(e.HireDate, DateTime.Now.AddMonths(-1));
                                     project.EndDate = f.Date.Between(project.StartDate, DateTime.Now);
                                     project.Technologies = f.PickRandom(e.Skills, f.Random.Int(1, Math.Min(4, e.Skills.Count))).ToList();
                                 }

                                 return projects;
                             });

    /// <summary>
    /// 產生多個產品
    /// </summary>
    public static List<Product> GenerateProducts(int count)
    {
        return ProductFaker.Generate(count);
    }

    /// <summary>
    /// 產生多個使用者
    /// </summary>
    public static List<User> GenerateUsers(int count)
    {
        return UserFaker.Generate(count);
    }

    /// <summary>
    /// 產生多個台灣人
    /// </summary>
    public static List<TaiwanPerson> GenerateTaiwanPeople(int count)
    {
        return TaiwanPersonFaker.Generate(count);
    }

    /// <summary>
    /// 產生多個訂單
    /// </summary>
    public static List<Order> GenerateOrders(int count)
    {
        return OrderFaker.Generate(count);
    }

    /// <summary>
    /// 產生多個員工
    /// </summary>
    public static List<Employee> GenerateEmployees(int count)
    {
        return EmployeeFaker.Generate(count);
    }
}