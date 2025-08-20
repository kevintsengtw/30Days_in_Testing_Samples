using Day11.Core.Tests.TestHelpers;

namespace Day11.Core.Tests;

/// <summary>
/// 泛型數值範圍建構器測試
/// </summary>
public class GenericNumericRangeBuilderTests
{
    [Fact]
    public void 使用擴充方法_控制不同數值型別的範圍()
    {
        // Arrange
        var fixture = new Fixture();

        // 控制不同型別的屬性範圍
        fixture.AddRandomRange<Product, decimal>(
            min: 100m,
            max: 1000m,
            predicate: prop => prop.Name == "Price" && prop.DeclaringType == typeof(Product));

        fixture.AddRandomRange<Product, int>(
            min: 1,
            max: 100,
            predicate: prop => prop.Name == "Quantity" && prop.DeclaringType == typeof(Product));

        // Act
        var product = fixture.Create<Product>();

        // Assert
        product.Price.Should().BeInRange(100m, 1000m);
        product.Quantity.Should().BeInRange(1, 99); // Next 不包含上限
    }

    [Fact]
    public void 複雜實體多重數值型別範圍控制_完整測試案例()
    {
        // Arrange
        var fixture = new Fixture();

        // Product 屬性範圍設定
        fixture.AddRandomRange<Product, decimal>(
            min: 50m,
            max: 500m,
            predicate: prop => prop.Name == "Price" && prop.DeclaringType == typeof(Product));
        fixture.AddRandomRange<Product, int>(
            min: 1,
            max: 50,
            predicate: prop => prop.Name == "Quantity" && prop.DeclaringType == typeof(Product));
        fixture.AddRandomRange<Product, double>(
            min: 1.0,
            max: 5.0,
            predicate: prop => prop.Name == "Rating" && prop.DeclaringType == typeof(Product));
        fixture.AddRandomRange<Product, float>(
            min: 0.0f,
            max: 0.5f,
            predicate: prop => prop.Name == "Discount" && prop.DeclaringType == typeof(Product));

        // Order 屬性範圍設定
        fixture.AddRandomRange<Order, decimal>(
            min: 100m,
            max: 10000m,
            predicate: prop => prop.Name == "Amount" && prop.DeclaringType == typeof(Order));
        fixture.AddRandomRange<Order, short>(
            min: (short)1,
            max: (short)20,
            predicate: prop => prop.Name == "ItemCount" && prop.DeclaringType == typeof(Order));
        fixture.AddRandomRange<Order, byte>(
            min: (byte)1,
            max: (byte)5,
            predicate: prop => prop.Name == "Priority" && prop.DeclaringType == typeof(Order));
        fixture.AddRandomRange<Order, long>(
            min: 1000L,
            max: 9999L,
            predicate: prop => prop.Name == "CustomerId" && prop.DeclaringType == typeof(Order));

        // Act
        var products = fixture.CreateMany<Product>(10).ToList();
        var orders = fixture.CreateMany<Order>(10).ToList();

        // Assert
        products.Should().AllSatisfy(product =>
        {
            product.Price.Should().BeInRange(50m, 500m);
            product.Quantity.Should().BeInRange(1, 49);
            product.Rating.Should().BeInRange(1.0, 5.0);
            product.Discount.Should().BeInRange(0.0f, 0.5f);
        });

        orders.Should().AllSatisfy(order =>
        {
            order.Amount.Should().BeInRange(100m, 10000m);
            order.ItemCount.Should().BeInRange((short)1, (short)19);
            order.Priority.Should().BeInRange((byte)1, (byte)4);
            order.CustomerId.Should().BeInRange(1000L, 9998L);
        });
    }

    [Fact]
    public void 泛型建構器_驗證各種數值型別支援()
    {
        // Arrange
        var fixture = new Fixture();

        // 設定各種數值型別的範圍
        fixture.AddRandomRange<Product, decimal>(
            min: 10.5m,
            max: 99.9m,
            predicate: prop => prop.Name == "Price");
        fixture.AddRandomRange<Product, int>(
            min: 5,
            max: 15,
            predicate: prop => prop.Name == "Quantity");
        fixture.AddRandomRange<Product, double>(
            min: 2.0,
            max: 4.0,
            predicate: prop => prop.Name == "Rating");
        fixture.AddRandomRange<Product, float>(
            min: 0.1f,
            max: 0.3f,
            predicate: prop => prop.Name == "Discount");

        fixture.AddRandomRange<Order, short>(
            min: (short)3,
            max: (short)8,
            predicate: prop => prop.Name == "ItemCount");
        fixture.AddRandomRange<Order, byte>(
            min: (byte)2,
            max: (byte)4,
            predicate: prop => prop.Name == "Priority");
        fixture.AddRandomRange<Order, long>(
            min: 2000L,
            max: 3000L,
            predicate: prop => prop.Name == "CustomerId");

        // Act
        var product = fixture.Create<Product>();
        var order = fixture.Create<Order>();

        // Assert
        // Product 屬性驗證
        product.Price.Should().BeInRange(10.5m, 99.9m);
        product.Quantity.Should().BeInRange(5, 14);
        product.Rating.Should().BeInRange(2.0, 4.0);
        product.Discount.Should().BeInRange(0.1f, 0.3f);

        // Order 屬性驗證
        order.ItemCount.Should().BeInRange((short)3, (short)7);
        order.Priority.Should().BeInRange((byte)2, (byte)3);
        order.CustomerId.Should().BeInRange(2000L, 2999L);
    }

    [Fact]
    public void 泛型建構器_產生多個物件時隨機性驗證()
    {
        // Arrange
        var fixture = new Fixture();
        fixture.AddRandomRange<Product, decimal>(
            min: 100m,
            max: 200m,
            predicate: prop => prop.Name == "Price");

        // Act
        var products = fixture.CreateMany<Product>(20).ToList();

        // Assert
        products.Should().HaveCount(20);
        products.Should().AllSatisfy(p => p.Price.Should().BeInRange(100m, 200m));

        // 驗證隨機性：20 個產品應該有多個不同的價格
        var distinctPrices = products.Select(p => p.Price).Distinct().Count();
        distinctPrices.Should().BeGreaterThan(5);
    }

    [Fact]
    public void NumericRangeBuilder_應正確回傳NoSpecimen_當不符合條件時()
    {
        // Arrange
        var builder = new NumericRangeBuilder<int>(
            min: 1,
            max: 100,
            predicate: prop => prop.Name == "Quantity");

        var context = new SpecimenContext(new Fixture());

        // Act & Assert - 非目標屬性應回傳 NoSpecimen
        var nameProperty = typeof(Product).GetProperty("Name");
        var result1 = builder.Create(nameProperty!, context);
        result1.Should().BeOfType<NoSpecimen>();

        // 數值型別但非目標屬性也應回傳 NoSpecimen
        var priceProperty = typeof(Product).GetProperty("Price");
        var result2 = builder.Create(priceProperty!, context);
        result2.Should().BeOfType<NoSpecimen>();

        // 目標屬性應回傳 int
        var quantityProperty = typeof(Product).GetProperty("Quantity");
        var result3 = builder.Create(quantityProperty!, context);
        result3.Should().BeOfType<int>();
        ((int)result3).Should().BeInRange(1, 99);
    }
}