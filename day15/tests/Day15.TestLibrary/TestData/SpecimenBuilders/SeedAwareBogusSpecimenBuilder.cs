using AutoFixture.Kernel;
using Bogus;
using Day15.Core.Models;

namespace Day15.TestLibrary.TestData.SpecimenBuilders;

/// <summary>
/// class SeedAwareBogusSpecimenBuilder
/// </summary>
/// <remarks>
/// 這個 SpecimenBuilder 使用 Bogus 來根據請求的型別產生對應的物件。<br/>
/// 根據所給的 seed，產生出可預期、可重現的假資料。<br/>
/// 如果 seed 不同，產生的資料也會跟著改變。這對測試很有幫助。
/// </remarks>
public class SeedAwareBogusSpecimenBuilder : ISpecimenBuilder
{
    private readonly Dictionary<Type, object> _fakers;
    private readonly int? _seed;

    /// <summary>
    /// 建構式，接受可選的 Seed
    /// </summary>
    /// <param name="seed">Seed</param>
    public SeedAwareBogusSpecimenBuilder(int? seed = null)
    {
        this._seed = seed;
        this._fakers = new Dictionary<Type, object>();
        this.RegisterFakers();
    }

    /// <summary>
    /// 根據請求的型別產生對應的物件
    /// </summary>
    /// <param name="request">request</param>
    /// <param name="context">context</param>
    /// <returns></returns>
    public object Create(object request, ISpecimenContext context)
    {
        if (request is Type type && this._fakers.TryGetValue(type, out var faker))
        {
            return this.GenerateWithFaker(faker);
        }

        return new NoSpecimen();
    }

    /// <summary>
    /// 註冊各種模型的 Faker
    /// </summary>
    private void RegisterFakers()
    {
        // 如果有 Seed，設定 Bogus 的 Random Seed
        if (this._seed.HasValue)
        {
            Randomizer.Seed = new Random(this._seed.Value);
        }

        // 註冊使用者相關的 Faker
        this._fakers[typeof(User)] = new Faker<User>().UseSeed(this._seed ?? Random.Shared.Next())
                                                      .RuleFor(u => u.Id, f => f.Random.Guid())
                                                      .RuleFor(u => u.FirstName, f => f.Person.FirstName)
                                                      .RuleFor(u => u.LastName, f => f.Person.LastName)
                                                      .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
                                                      .RuleFor(u => u.BirthDate, f => f.Person.DateOfBirth)
                                                      .RuleFor(u => u.Age, f => f.Random.Int(18, 80))
                                                      .RuleFor(u => u.Phone, f => f.Phone.PhoneNumber())
                                                      .Ignore(u => u.HomeAddress)
                                                      .Ignore(u => u.Company)
                                                      .Ignore(u => u.Orders);

        // 註冊地址相關的 Faker
        this._fakers[typeof(Address)] = new Faker<Address>().UseSeed(this._seed ?? Random.Shared.Next())
                                                            .RuleFor(a => a.Id, f => f.Random.Guid())
                                                            .RuleFor(a => a.Street, f => f.Address.StreetAddress())
                                                            .RuleFor(a => a.City, f => f.Address.City())
                                                            .RuleFor(a => a.PostalCode, f => f.Address.ZipCode())
                                                            .RuleFor(a => a.Country, f => f.Address.Country());

        // 註冊公司相關的 Faker
        this._fakers[typeof(Company)] = new Faker<Company>().UseSeed(this._seed ?? Random.Shared.Next())
                                                            .RuleFor(c => c.Id, f => f.Random.Guid())
                                                            .RuleFor(c => c.Name, f => f.Company.CompanyName())
                                                            .RuleFor(c => c.Industry, f => f.Commerce.Department())
                                                            .RuleFor(c => c.Website, f => f.Internet.Url())
                                                            .RuleFor(c => c.Phone, f => f.Phone.PhoneNumber())
                                                            .Ignore(c => c.Address)
                                                            .Ignore(c => c.Employees);

        // 註冊產品相關的 Faker
        this._fakers[typeof(Product)] = new Faker<Product>().UseSeed(this._seed ?? Random.Shared.Next())
                                                            .RuleFor(p => p.Id, f => f.Random.Guid())
                                                            .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                                                            .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
                                                            .RuleFor(p => p.Price, f => f.Random.Decimal(1, 1000))
                                                            .RuleFor(p => p.Category, f => f.Commerce.Categories(1).First())
                                                            .RuleFor(p => p.IsActive, f => f.Random.Bool(0.8f));

        // 註冊訂單項目相關的 Faker
        this._fakers[typeof(OrderItem)] = new Faker<OrderItem>().UseSeed(this._seed ?? Random.Shared.Next())
                                                                .RuleFor(oi => oi.Id, f => f.Random.Guid())
                                                                .RuleFor(oi => oi.Quantity, f => f.Random.Int(1, 10))
                                                                .RuleFor(oi => oi.UnitPrice, f => f.Random.Decimal(1, 500))
                                                                .Ignore(oi => oi.Product);

        // 註冊訂單相關的 Faker
        this._fakers[typeof(Order)] = new Faker<Order>().UseSeed(this._seed ?? Random.Shared.Next())
                                                        .RuleFor(o => o.Id, f => f.Random.Guid())
                                                        .RuleFor(o => o.OrderDate, f => f.Date.Recent(30))
                                                        .RuleFor(o => o.TotalAmount, f => f.Random.Decimal(10, 5000))
                                                        .RuleFor(o => o.Status, f => f.Random.Enum<OrderStatus>())
                                                        .Ignore(o => o.Customer)
                                                        .Ignore(o => o.Items);
    }

    /// <summary>
    /// 使用 Bogus 的 Faker 來產生物件
    /// </summary>
    /// <param name="faker">faker</param>
    /// <returns></returns>
    private object GenerateWithFaker(object faker)
    {
        var generateMethod = faker.GetType().GetMethod("Generate", Type.EmptyTypes);
        return generateMethod?.Invoke(faker, null) ?? new NoSpecimen();
    }
}