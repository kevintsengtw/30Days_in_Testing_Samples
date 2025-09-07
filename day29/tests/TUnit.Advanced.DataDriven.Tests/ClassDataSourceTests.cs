using System.Collections;

namespace TUnit.Advanced.DataDriven.Tests;

/// <summary>
/// ClassDataSource 進階應用 - 展示複雜資料模型和依賴注入
/// </summary>
public class ClassDataSourceTests
{
    /// <summary>
    /// 使用 ClassDataSource 測試完整訂單計算流程
    /// </summary>
    [Test]
    [ClassDataSource(typeof(OrderCalculationTestData))]
    public async Task CalculateOrderTotal_使用複雜案例_應正確計算所有費用(OrderTestCase testCase)
    {
        // Arrange
        var discountCalculator = new DiscountCalculator(
            new MockDiscountRepository(),
            new MockLogger<DiscountCalculator>());
        var shippingCalculator = new ShippingCalculator();

        var order = new Order
        {
            OrderId = "TEST001",
            CustomerId = testCase.CustomerId,
            CustomerLevel = testCase.CustomerLevel,
            Items = testCase.Items
        };

        // Act - 計算折扣
        if (!string.IsNullOrEmpty(testCase.DiscountCode))
        {
            order.DiscountAmount = await discountCalculator.CalculateDiscountAsync(order, testCase.DiscountCode);
        }

        // Act - 計算運費
        order.ShippingFee = shippingCalculator.CalculateShippingFee(order);

        // Assert - 驗證所有計算結果
        await Assert.That(order.SubTotal).IsEqualTo(testCase.ExpectedSubTotal);
        await Assert.That(order.DiscountAmount).IsEqualTo(testCase.ExpectedDiscount);
        await Assert.That(order.ShippingFee).IsEqualTo(testCase.ExpectedShipping);
        await Assert.That(order.TotalAmount).IsEqualTo(testCase.ExpectedTotal);
    }

    /// <summary>
    /// 訂單測試案例資料類別
    /// </summary>
    public class OrderTestCase
    {
        public string TestName { get; set; } = string.Empty;
        public string CustomerId { get; set; } = string.Empty;
        public CustomerLevel CustomerLevel { get; set; }
        public List<OrderItem> Items { get; set; } = [];
        public string? DiscountCode { get; set; }
        public decimal ExpectedSubTotal { get; set; }
        public decimal ExpectedDiscount { get; set; }
        public decimal ExpectedShipping { get; set; }
        public decimal ExpectedTotal { get; set; }

        public override string ToString()
        {
            return TestName;
        }
    }

    /// <summary>
    /// 複雜訂單計算測試資料產生器
    /// </summary>
    public class OrderCalculationTestData : IEnumerable<OrderTestCase>
    {
        public IEnumerator<OrderTestCase> GetEnumerator()
        {
            // 一般會員基本訂單
            yield return new OrderTestCase
            {
                TestName = "一般會員_基本訂單_無折扣",
                CustomerId = "CUST001",
                CustomerLevel = CustomerLevel.一般會員,
                Items =
                [
                    new OrderItem { ProductId = "P001", ProductName = "商品A", UnitPrice = 500m, Quantity = 2 }
                ],
                ExpectedSubTotal = 1000m,
                ExpectedDiscount = 0m,
                ExpectedShipping = 0m, // 滿1000免運
                ExpectedTotal = 1000m
            };

            // VIP會員有折扣
            yield return new OrderTestCase
            {
                TestName = "VIP會員_有折扣碼_運費半價",
                CustomerId = "CUST002",
                CustomerLevel = CustomerLevel.VIP會員,
                Items =
                [
                    new OrderItem { ProductId = "P002", ProductName = "商品B", UnitPrice = 300m, Quantity = 2 }
                ],
                DiscountCode = "PERCENT10",
                ExpectedSubTotal = 600m,
                ExpectedDiscount = 60m,
                ExpectedShipping = 40m, // VIP運費半價
                ExpectedTotal = 580m
            };

            // 鑽石會員大量購買
            yield return new OrderTestCase
            {
                TestName = "鑽石會員_大量購買_免運免折扣",
                CustomerId = "CUST003",
                CustomerLevel = CustomerLevel.鑽石會員,
                Items =
                [
                    new OrderItem { ProductId = "P003", ProductName = "商品C", UnitPrice = 1500m, Quantity = 3 },
                    new OrderItem { ProductId = "P004", ProductName = "商品D", UnitPrice = 800m, Quantity = 2 }
                ],
                ExpectedSubTotal = 6100m,
                ExpectedDiscount = 0m,
                ExpectedShipping = 0m, // 鑽石會員免運
                ExpectedTotal = 6100m
            };

            // 邊界案例：小額訂單
            yield return new OrderTestCase
            {
                TestName = "一般會員_小額訂單_標準運費",
                CustomerId = "CUST004",
                CustomerLevel = CustomerLevel.一般會員,
                Items =
                [
                    new OrderItem { ProductId = "P005", ProductName = "商品E", UnitPrice = 100m, Quantity = 1 }
                ],
                ExpectedSubTotal = 100m,
                ExpectedDiscount = 0m,
                ExpectedShipping = 80m,
                ExpectedTotal = 180m
            };
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

/// <summary>
/// 使用 AutoFixture 產生測試資料的 ClassDataSource 範例
/// </summary>
public class AutoFixtureDataSourceTests
{
    /// <summary>
    /// 使用 AutoFixture 產生的資料測試訂單處理
    /// </summary>
    [Test]
    [ClassDataSource(typeof(AutoFixtureOrderTestData))]
    public async Task ProcessOrder_自動產生測試資料_應正確計算訂單金額(Order order)
    {
        // Arrange
        var discountCalculator = new DiscountCalculator(
            new MockDiscountRepository(),
            new MockLogger<DiscountCalculator>());
        var shippingCalculator = new ShippingCalculator();

        // Act - 實際使用 calculator 計算折扣和運費
        var discountAmount = await discountCalculator.CalculateDiscountAsync(order, "PERCENT10");
        var shippingFee = shippingCalculator.CalculateShippingFee(order);

        // 更新訂單金額
        order.DiscountAmount = discountAmount;
        order.ShippingFee = shippingFee;

        // Assert - 驗證隨機產生的資料能正常處理
        await Assert.That(order).IsNotNull();
        await Assert.That(order.CustomerId).IsNotEmpty();
        await Assert.That(order.Items).IsNotEmpty();

        // 驗證所有項目都有有效的資料
        foreach (var item in order.Items)
        {
            await Assert.That(item.ProductId).IsNotEmpty();
            await Assert.That(item.ProductName).IsNotEmpty();
            await Assert.That(item.UnitPrice).IsGreaterThan(0);
            await Assert.That(item.Quantity).IsGreaterThan(0);
        }

        // 驗證計算結果的合理性
        var expectedSubTotal = order.Items.Sum(i => i.UnitPrice * i.Quantity);
        await Assert.That(order.SubTotal).IsEqualTo(expectedSubTotal);

        // 驗證折扣計算（PERCENT10 應該是 10% 折扣）
        var expectedDiscount = order.SubTotal * 0.1m;
        await Assert.That(order.DiscountAmount).IsEqualTo(expectedDiscount);

        // 驗證運費計算不為負數
        await Assert.That(order.ShippingFee).IsGreaterThanOrEqualTo(0);

        // 驗證總金額計算正確
        var expectedTotal = order.SubTotal - order.DiscountAmount + order.ShippingFee;
        await Assert.That(order.TotalAmount).IsEqualTo(expectedTotal);
    }

    /// <summary>
    /// 使用 AutoFixture 產生訂單測試資料
    /// </summary>
    public class AutoFixtureOrderTestData : IEnumerable<Order>
    {
        private readonly Fixture _fixture;

        public AutoFixtureOrderTestData()
        {
            _fixture = new Fixture();

            // 自訂 Order 的產生規則
            _fixture.Customize<Order>(
                composer => composer.With(o => o.CustomerId, () => $"CUST{_fixture.Create<int>() % 1000:D3}")
                                    .With(o => o.CustomerLevel, () => _fixture.Create<CustomerLevel>())
                                    .With(o => o.Items, () => _fixture.CreateMany<OrderItem>(Random.Shared.Next(1, 5)).ToList()));

            // 自訂 OrderItem 的產生規則
            _fixture.Customize<OrderItem>(
                composer => composer.With(oi => oi.ProductId, () => $"PROD{_fixture.Create<int>() % 1000:D3}")
                                    .With(oi => oi.ProductName, () => $"測試商品{_fixture.Create<int>() % 100}")
                                    .With(oi => oi.UnitPrice, () => Math.Round(_fixture.Create<decimal>() % 1000 + 1, 2))
                                    .With(oi => oi.Quantity, () => _fixture.Create<int>() % 10 + 1));
        }

        public IEnumerator<Order> GetEnumerator()
        {
            // 產生多個隨機訂單進行測試
            for (var i = 0; i < 5; i++)
            {
                yield return _fixture.Create<Order>();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    /// <summary>
    /// 使用 AutoFixture 產生的資料測試項目總價計算
    /// </summary>
    [Test]
    [ClassDataSource(typeof(AutoFixtureOrderItemData))]
    public async Task CalculateItemTotalPrice_使用隨機資料_應正確計算(OrderItem item)
    {
        // Arrange & Act
        var totalPrice = item.TotalPrice;
        var expectedTotal = item.UnitPrice * item.Quantity;

        // Assert
        await Assert.That(totalPrice).IsEqualTo(expectedTotal);
        await Assert.That(item.UnitPrice).IsGreaterThan(0);
        await Assert.That(item.Quantity).IsGreaterThan(0);
        await Assert.That(item.ProductId).IsNotEmpty();
        await Assert.That(item.ProductName).IsNotEmpty();
    }

    /// <summary>
    /// 使用 AutoFixture 產生訂單項目測試資料
    /// </summary>
    public class AutoFixtureOrderItemData : IEnumerable<OrderItem>
    {
        private readonly Fixture _fixture = new();

        public AutoFixtureOrderItemData()
        {
            // 自訂 AutoFixture 產生規則
            _fixture.Customize<OrderItem>(
                composer => composer.With(x => x.ProductId, () => $"P{_fixture.Create<int>() % 1000:D3}")
                                    .With(x => x.ProductName, () => $"測試商品{_fixture.Create<string>()[..5]}")
                                    .With(x => x.UnitPrice, () => Math.Round(_fixture.Create<decimal>() % 10000, 2))
                                    .With(x => x.Quantity, () => _fixture.Create<int>() % 10 + 1)
            );
        }

        public IEnumerator<OrderItem> GetEnumerator()
        {
            // 產生 5 個隨機的訂單項目作為測試資料
            for (var i = 0; i < 5; i++)
            {
                yield return _fixture.Create<OrderItem>();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}