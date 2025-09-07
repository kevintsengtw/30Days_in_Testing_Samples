namespace TUnit.Advanced.DataDriven.Tests;

/// <summary>
/// Matrix Tests 展示組合測試的威力與注意事項
/// 
/// Matrix Tests 的優點：
/// 1. 自動產生所有參數組合，確保測試覆蓋率
/// 2. 大幅減少手動撰寫測試案例的工作量
/// 3. 適合測試多維度參數的交互作用
/// 
/// Matrix Tests 的注意事項：
/// 1. 測試數量爆炸：n個參數各有m個值 = m^n 個測試案例
/// 2. enum 在屬性中必須使用數值表示（C# 限制）
/// 3. 需要謹慎控制組合數量，避免測試執行時間過長
/// 4. 可以使用 MatrixExclusion 排除特定組合
/// </summary>
public class MatrixTestsExamples
{
    /// <summary>
    /// 基本 Matrix Test：客戶等級 × 訂單金額的組合測試
    /// 這個測試會產生 4 × 4 = 16 個測試案例
    /// 注意：由於 C# 屬性限制，enum 必須用數值表示：0=一般會員, 1=VIP會員, 2=白金會員, 3=鑽石會員
    /// </summary>
    [Test]
    [MatrixDataSource]
    public async Task CalculateShipping_客戶等級與金額組合_應遵循運費規則(
        [Matrix(0, 1, 2, 3)] CustomerLevel customerLevel, // 0=一般會員, 1=VIP會員, 2=白金會員, 3=鑽石會員
        [Matrix(100, 500, 1000, 2000)] decimal orderAmount)
    {
        // Arrange
        var calculator = new ShippingCalculator();
        var order = new Order
        {
            CustomerLevel = customerLevel,
            Items = [new OrderItem { UnitPrice = orderAmount, Quantity = 1 }]
        };

        // Act
        var shippingFee = calculator.CalculateShippingFee(order);
        var isFreeShipping = calculator.IsEligibleForFreeShipping(order);

        // Assert - 驗證運費邏輯的一致性
        if (isFreeShipping)
        {
            await Assert.That(shippingFee).IsEqualTo(0m);
        }
        else
        {
            await Assert.That(shippingFee).IsGreaterThan(0m);
        }

        // 驗證特定規則
        switch (customerLevel)
        {
            case CustomerLevel.鑽石會員:
                await Assert.That(shippingFee).IsEqualTo(0m); // 鑽石會員永遠免運
                break;

            case CustomerLevel.VIP會員 or CustomerLevel.白金會員:
                if (orderAmount < 1000m)
                {
                    await Assert.That(shippingFee).IsEqualTo(40m); // VIP+ 運費半價
                }

                break;

            case CustomerLevel.一般會員:
                if (orderAmount < 1000m)
                {
                    await Assert.That(shippingFee).IsEqualTo(80m); // 一般會員標準運費
                }

                break;
        }
    }

    /// <summary>
    /// 複雜 Matrix Test：三個維度的組合測試
    /// 注意：這會產生 4 × 4 × 3 = 48 個測試案例！
    /// 在實務上要謹慎使用，避免測試爆炸
    /// </summary>
    [Test]
    [MatrixDataSource]
    public async Task CompleteOrderCalculation_三維度組合測試_應保持計算一致性(
        [Matrix(0, 1, 2, 3)] CustomerLevel customerLevel, // 0=一般會員, 1=VIP會員, 2=白金會員, 3=鑽石會員
        [Matrix(100, 500, 1000, 2000)] decimal orderAmount,
        [Matrix("", "PERCENT10", "PERCENT20")] string discountCode)
    {
        // Arrange
        var discountCalculator = new DiscountCalculator(
            new MockDiscountRepository(),
            new MockLogger<DiscountCalculator>());
        var shippingCalculator = new ShippingCalculator();

        var order = new Order
        {
            OrderId = $"TEST_{customerLevel}_{orderAmount}_{discountCode ?? "NODISCOUNT"}",
            CustomerLevel = customerLevel,
            Items = [new OrderItem { UnitPrice = orderAmount, Quantity = 1 }]
        };

        // Act
        var discountAmount = string.IsNullOrEmpty(discountCode)
            ? 0m
            : await discountCalculator.CalculateDiscountAsync(order, discountCode);

        order.DiscountAmount = discountAmount;
        order.ShippingFee = shippingCalculator.CalculateShippingFee(order);

        // Assert - 基本邏輯驗證
        await Assert.That(order.SubTotal).IsEqualTo(orderAmount);
        await Assert.That(order.DiscountAmount).IsGreaterThanOrEqualTo(0m);
        await Assert.That(order.ShippingFee).IsGreaterThanOrEqualTo(0m);

        // 確保總金額計算正確
        var expectedTotal = order.SubTotal - order.DiscountAmount + order.ShippingFee;
        await Assert.That(order.TotalAmount).IsEqualTo(expectedTotal);

        // 確保折扣不會超過訂單金額
        await Assert.That(order.DiscountAmount).IsLessThanOrEqualTo(order.SubTotal);
    }

    /// <summary>
    /// 有條件的 Matrix Test：使用測試屬性進行過濾
    /// 展示如何控制 Matrix 測試的規模
    /// </summary>
    [Test]
    [MatrixDataSource]
    public async Task VipCustomerHighValueOrder_限制組合範圍_應享有特殊待遇(
        [Matrix(1, 2, 3)] CustomerLevel customerLevel, // 1=VIP會員, 2=白金會員, 3=鑽石會員
        [Matrix(1000, 2000, 5000)] decimal orderAmount)
    {
        // Arrange
        var shippingCalculator = new ShippingCalculator();
        var order = new Order
        {
            CustomerLevel = customerLevel,
            Items = [new OrderItem { UnitPrice = orderAmount, Quantity = 1 }]
        };

        // Act
        var shippingFee = shippingCalculator.CalculateShippingFee(order);

        // Assert - VIP 以上客戶的高價訂單應該免運費
        await Assert.That(shippingFee).IsEqualTo(0m);
    }

    /// <summary>
    /// 使用 bool 類型的 Matrix 測試
    /// 展示如何根據布林值進行不同的業務邏輯測試
    /// </summary>
    [Test]
    [MatrixDataSource]
    public async Task BooleanMatrix_展示布林值組合測試(
        [Matrix(0, 1)] CustomerLevel level, // 0=一般會員, 1=VIP會員
        [Matrix] bool hasDiscount)          // 無參數的 Matrix 會自動產生 true/false
    {
        // Arrange
        var discountCalculator = new DiscountCalculator(
            new MockDiscountRepository(),
            new MockLogger<DiscountCalculator>());

        var order = new Order
        {
            CustomerLevel = level,
            Items = [new OrderItem { UnitPrice = 500m, Quantity = 1 }]
        };

        // Act - 根據 hasDiscount 決定是否套用折扣
        var discountAmount = 0m;
        if (hasDiscount)
        {
            discountAmount = await discountCalculator.CalculateDiscountAsync(order, "PERCENT10");
        }

        // Assert - 根據不同的布林值驗證不同的業務邏輯
        if (hasDiscount)
        {
            await Assert.That(discountAmount).IsGreaterThan(0m);
            // VIP 會員應該享有更多折扣優惠
            if (level == CustomerLevel.VIP會員)
            {
                await Assert.That(discountAmount).IsEqualTo(50m); // 10% of 500
            }
        }
        else
        {
            await Assert.That(discountAmount).IsEqualTo(0m);
        }

        // 驗證客戶等級的基本屬性
        await Assert.That((int)level).IsGreaterThanOrEqualTo(0);
        await Assert.That((int)level).IsLessThanOrEqualTo(3);
    }
}

/// <summary>
/// Matrix Tests 最佳實務範例
/// </summary>
public class MatrixTestBestPractices
{
    /// <summary>
    /// 良好實務：有限的組合數量，專注核心邏輯
    /// 使用 Arguments 的組合只會產生 2 × 2 = 4 個測試案例
    /// </summary>
    [Test]
    [MatrixDataSource]
    public async Task ShippingCalculation_核心邏輯驗證_有限組合(
        [Matrix(0, 1)] CustomerLevel level, // 0=一般會員, 1=VIP會員
        [Matrix(100, 1000)] decimal amount)
    {
        // Arrange
        var calculator = new ShippingCalculator();
        var order = new Order
        {
            CustomerLevel = level,
            Items = [new OrderItem { UnitPrice = amount, Quantity = 1 }]
        };

        // Act
        var fee = calculator.CalculateShippingFee(order);

        // Assert
        if (amount >= 1000m || level == CustomerLevel.鑽石會員)
        {
            await Assert.That(fee).IsEqualTo(0m);
        }
        else
        {
            await Assert.That(fee).IsGreaterThan(0m);
        }
    }

    /// <summary>
    /// 展示字串與數字的 Matrix 組合
    /// </summary>
    [Test]
    [MatrixDataSource]
    public async Task StringAndNumberMatrix_多型別組合測試(
        [Matrix("A", "B", "C")] string productCode,
        [Matrix(1, 2, 3)] int quantity)
    {
        // Arrange & Act
        var item = new OrderItem
        {
            ProductId = productCode,
            ProductName = $"Product {productCode}",
            UnitPrice = 100m,
            Quantity = quantity
        };

        // Assert
        await Assert.That(item.ProductId).IsEqualTo(productCode);
        await Assert.That(item.Quantity).IsEqualTo(quantity);
        await Assert.That(item.TotalPrice).IsEqualTo(100m * quantity);
    }

    /// <summary>
    /// 展示 MatrixExclusion 的使用 - 排除特定組合
    /// 這個測試展示如何排除不合理的組合，例如鑽石會員使用低價產品的情況
    /// </summary>
    [Test]
    [MatrixDataSource]
    [MatrixExclusion(3, 50)]  // 排除鑽石會員(3) + 低價產品(50)的組合
    [MatrixExclusion(3, 100)] // 排除鑽石會員(3) + 中價產品(100)的組合
    public async Task MatrixWithExclusions_排除不合理組合_確保邏輯一致性(
        [Matrix(0, 1, 2, 3)] CustomerLevel customerLevel, // 0=一般, 1=VIP, 2=白金, 3=鑽石
        [Matrix(50, 100, 500, 1000)] decimal productPrice)
    {
        // Arrange
        var order = new Order
        {
            CustomerLevel = customerLevel,
            Items = [new OrderItem { UnitPrice = productPrice, Quantity = 1 }]
        };

        // Act
        var shippingFee = new ShippingCalculator().CalculateShippingFee(order);

        // Assert
        // 由於排除了鑽石會員的低價產品組合，所以這裡的邏輯會更加一致
        if (customerLevel == CustomerLevel.鑽石會員)
        {
            // 鑽石會員應該只購買高價產品（因為低價產品組合已被排除）
            await Assert.That(productPrice).IsGreaterThanOrEqualTo(500m);
            await Assert.That(shippingFee).IsEqualTo(0m); // 鑽石會員永遠免運
        }
        else if (productPrice >= 1000m)
        {
            await Assert.That(shippingFee).IsEqualTo(0m); // 高價產品免運
        }
        else
        {
            await Assert.That(shippingFee).IsGreaterThan(0m); // 其他情況需要運費
        }
    }
}