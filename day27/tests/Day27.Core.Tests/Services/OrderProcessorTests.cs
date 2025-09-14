namespace Day27.Core.Tests.Services;

/// <summary>
/// 訂單處理服務測試類別
/// </summary>
public class OrderProcessorTests
{
    private readonly IOrderRepository mockOrderRepository;
    private readonly IPaymentService mockPaymentService;
    private readonly IInventoryService mockInventoryService;
    private readonly INotificationService mockNotificationService;
    private readonly OrderProcessor orderProcessor;

    /// <summary>
    /// 建構式：初始化測試環境
    /// </summary>
    public OrderProcessorTests()
    {
        mockOrderRepository = Substitute.For<IOrderRepository>();
        mockPaymentService = Substitute.For<IPaymentService>();
        mockInventoryService = Substitute.For<IInventoryService>();
        mockNotificationService = Substitute.For<INotificationService>();
        orderProcessor = new OrderProcessor(
            mockOrderRepository,
            mockPaymentService,
            mockInventoryService,
            mockNotificationService);
    }

    // CancelOrderAsync 取消訂單

    #region 正常情境測試

    [Fact(DisplayName = "取消訂單: 取消待處理狀態的訂單，應成功更新狀態為已取消")]
    public async Task CancelOrderAsync_取消待處理狀態的訂單_應成功更新狀態為已取消()
    {
        // Arrange
        var orderId = 1;
        var order = new Order
        {
            Id = orderId,
            CustomerId = 100,
            ProductId = 200,
            Quantity = 2,
            UnitPrice = 50m,
            PaymentMethod = "CreditCard",
            Status = OrderStatus.Pending
        };

        var cancelledOrder = new Order
        {
            Id = orderId,
            CustomerId = 100,
            ProductId = 200,
            Quantity = 2,
            UnitPrice = 50m,
            PaymentMethod = "CreditCard",
            Status = OrderStatus.Cancelled
        };

        mockOrderRepository.GetByIdAsync(orderId).Returns(order);
        mockOrderRepository.SaveAsync(Arg.Any<Order>()).Returns(cancelledOrder);

        // Act
        var result = await orderProcessor.CancelOrderAsync(orderId);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(OrderStatus.Cancelled);
        result.Id.Should().Be(orderId);
        await mockOrderRepository.Received(1).GetByIdAsync(orderId);
        await mockOrderRepository.Received(1).SaveAsync(Arg.Is<Order>(o => o.Status == OrderStatus.Cancelled));
    }

    [Fact(DisplayName = "取消訂單: 取消已確認狀態的訂單，應成功更新狀態為已取消")]
    public async Task CancelOrderAsync_取消已確認狀態的訂單_應成功更新狀態為已取消()
    {
        // Arrange
        var orderId = 2;
        var order = new Order
        {
            Id = orderId,
            CustomerId = 101,
            ProductId = 201,
            Quantity = 1,
            UnitPrice = 100m,
            PaymentMethod = "BankTransfer",
            Status = OrderStatus.Confirmed
        };

        var cancelledOrder = new Order
        {
            Id = orderId,
            CustomerId = 101,
            ProductId = 201,
            Quantity = 1,
            UnitPrice = 100m,
            PaymentMethod = "BankTransfer",
            Status = OrderStatus.Cancelled
        };

        mockOrderRepository.GetByIdAsync(orderId).Returns(order);
        mockOrderRepository.SaveAsync(Arg.Any<Order>()).Returns(cancelledOrder);

        // Act
        var result = await orderProcessor.CancelOrderAsync(orderId);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(OrderStatus.Cancelled);
        result.Id.Should().Be(orderId);
        await mockOrderRepository.Received(1).GetByIdAsync(orderId);
        await mockOrderRepository.Received(1).SaveAsync(Arg.Is<Order>(o => o.Status == OrderStatus.Cancelled));
    }

    #endregion

    #region 邊界條件測試

    [Fact(DisplayName = "取消訂單: 使用最小有效訂單編號，應成功取消訂單")]
    public async Task CancelOrderAsync_使用最小有效訂單編號_應成功取消訂單()
    {
        // Arrange
        var orderId = 1;
        var order = new Order
        {
            Id = orderId,
            CustomerId = 100,
            ProductId = 200,
            Quantity = 1,
            UnitPrice = 10m,
            PaymentMethod = "Cash",
            Status = OrderStatus.Pending
        };

        var cancelledOrder = new Order
        {
            Id = orderId,
            CustomerId = 100,
            ProductId = 200,
            Quantity = 1,
            UnitPrice = 10m,
            PaymentMethod = "Cash",
            Status = OrderStatus.Cancelled
        };

        mockOrderRepository.GetByIdAsync(orderId).Returns(order);
        mockOrderRepository.SaveAsync(Arg.Any<Order>()).Returns(cancelledOrder);

        // Act
        var result = await orderProcessor.CancelOrderAsync(orderId);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(OrderStatus.Cancelled);
        result.Id.Should().Be(orderId);
        await mockOrderRepository.Received(1).GetByIdAsync(orderId);
        await mockOrderRepository.Received(1).SaveAsync(Arg.Is<Order>(o => o.Status == OrderStatus.Cancelled));
    }

    [Fact(DisplayName = "取消訂單: 使用大數值訂單編號，應成功取消訂單")]
    public async Task CancelOrderAsync_使用大數值訂單編號_應成功取消訂單()
    {
        // Arrange
        var orderId = 999999;
        var order = new Order
        {
            Id = orderId,
            CustomerId = 500,
            ProductId = 300,
            Quantity = 5,
            UnitPrice = 200m,
            PaymentMethod = "CreditCard",
            Status = OrderStatus.Pending
        };

        var cancelledOrder = new Order
        {
            Id = orderId,
            CustomerId = 500,
            ProductId = 300,
            Quantity = 5,
            UnitPrice = 200m,
            PaymentMethod = "CreditCard",
            Status = OrderStatus.Cancelled
        };

        mockOrderRepository.GetByIdAsync(orderId).Returns(order);
        mockOrderRepository.SaveAsync(Arg.Any<Order>()).Returns(cancelledOrder);

        // Act
        var result = await orderProcessor.CancelOrderAsync(orderId);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(OrderStatus.Cancelled);
        result.Id.Should().Be(orderId);
        await mockOrderRepository.Received(1).GetByIdAsync(orderId);
        await mockOrderRepository.Received(1).SaveAsync(Arg.Is<Order>(o => o.Status == OrderStatus.Cancelled));
    }

    #endregion

    #region 異常處理測試

    [Fact(DisplayName = "取消訂單: 訂單不存在，應拋出 ArgumentException")]
    public async Task CancelOrderAsync_訂單不存在_應拋出ArgumentException()
    {
        // Arrange
        var orderId = 999;
        mockOrderRepository.GetByIdAsync(orderId).Returns((Order?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => orderProcessor.CancelOrderAsync(orderId));

        exception.Message.Should().Be("訂單不存在 (Parameter 'orderId')");
        exception.ParamName.Should().Be("orderId");
        await mockOrderRepository.Received(1).GetByIdAsync(orderId);
        await mockOrderRepository.DidNotReceive().SaveAsync(Arg.Any<Order>());
    }

    [Fact(DisplayName = "取消訂單: 嘗試取消已付款的訂單，應拋出 InvalidOperationException")]
    public async Task CancelOrderAsync_嘗試取消已付款的訂單_應拋出InvalidOperationException()
    {
        // Arrange
        var orderId = 3;
        var order = new Order
        {
            Id = orderId,
            CustomerId = 102,
            ProductId = 202,
            Quantity = 3,
            UnitPrice = 75m,
            PaymentMethod = "CreditCard",
            Status = OrderStatus.Paid
        };

        mockOrderRepository.GetByIdAsync(orderId).Returns(order);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => orderProcessor.CancelOrderAsync(orderId));

        exception.Message.Should().Be("已付款的訂單無法取消");
        await mockOrderRepository.Received(1).GetByIdAsync(orderId);
        await mockOrderRepository.DidNotReceive().SaveAsync(Arg.Any<Order>());
    }

    [Fact(DisplayName = "取消訂單: 嘗試取消已取消的訂單，應拋出 InvalidOperationException")]
    public async Task CancelOrderAsync_嘗試取消已取消的訂單_應拋出InvalidOperationException()
    {
        // Arrange
        var orderId = 4;
        var order = new Order
        {
            Id = orderId,
            CustomerId = 103,
            ProductId = 203,
            Quantity = 2,
            UnitPrice = 120m,
            PaymentMethod = "BankTransfer",
            Status = OrderStatus.Cancelled
        };

        mockOrderRepository.GetByIdAsync(orderId).Returns(order);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => orderProcessor.CancelOrderAsync(orderId));

        exception.Message.Should().Be("訂單已被取消");
        await mockOrderRepository.Received(1).GetByIdAsync(orderId);
        await mockOrderRepository.DidNotReceive().SaveAsync(Arg.Any<Order>());
    }

    [Fact(DisplayName = "取消訂單: 資料庫操作失敗，應拋出相應的資料庫異常")]
    public async Task CancelOrderAsync_資料庫操作失敗_應拋出相應的資料庫異常()
    {
        // Arrange
        var orderId = 5;
        var order = new Order
        {
            Id = orderId,
            CustomerId = 104,
            ProductId = 204,
            Quantity = 1,
            UnitPrice = 300m,
            PaymentMethod = "CreditCard",
            Status = OrderStatus.Pending
        };

        var databaseException = new InvalidOperationException("資料庫連線失敗");
        mockOrderRepository.GetByIdAsync(orderId).Returns(order);
        mockOrderRepository.When(x => x.SaveAsync(Arg.Any<Order>())).Do(x => throw databaseException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => orderProcessor.CancelOrderAsync(orderId));

        exception.Message.Should().Be("資料庫連線失敗");
        await mockOrderRepository.Received(1).GetByIdAsync(orderId);
        await mockOrderRepository.Received(1).SaveAsync(Arg.Is<Order>(o => o.Status == OrderStatus.Cancelled));
    }

    [Fact(DisplayName = "取消訂單: 負數訂單編號，應拋出 ArgumentException")]
    public async Task CancelOrderAsync_負數訂單編號_應拋出ArgumentException()
    {
        // Arrange
        var orderId = -1;
        mockOrderRepository.GetByIdAsync(orderId).Returns((Order?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => orderProcessor.CancelOrderAsync(orderId));

        exception.Message.Should().Be("訂單不存在 (Parameter 'orderId')");
        exception.ParamName.Should().Be("orderId");
        await mockOrderRepository.Received(1).GetByIdAsync(orderId);
        await mockOrderRepository.DidNotReceive().SaveAsync(Arg.Any<Order>());
    }

    [Fact(DisplayName = "取消訂單: 零值訂單編號，應拋出 ArgumentException")]
    public async Task CancelOrderAsync_零值訂單編號_應拋出ArgumentException()
    {
        // Arrange
        var orderId = 0;
        mockOrderRepository.GetByIdAsync(orderId).Returns((Order?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => orderProcessor.CancelOrderAsync(orderId));

        exception.Message.Should().Be("訂單不存在 (Parameter 'orderId')");
        exception.ParamName.Should().Be("orderId");
        await mockOrderRepository.Received(1).GetByIdAsync(orderId);
        await mockOrderRepository.DidNotReceive().SaveAsync(Arg.Any<Order>());
    }

    #endregion

    // ProcessOrderAsync 處理訂單

    #region 正常情境測試

    [Fact(DisplayName = "處理訂單: 完整成功的訂單處理流程，應成功確認訂單")]
    public async Task ProcessOrderAsync_完整成功的訂單處理流程_應成功確認訂單()
    {
        // Arrange
        var order = new Order
        {
            Id = 1,
            CustomerId = 100,
            ProductId = 200,
            Quantity = 2,
            UnitPrice = 50m,
            PaymentMethod = "CreditCard",
            Status = OrderStatus.Pending
        };

        var confirmedOrder = new Order
        {
            Id = 1,
            CustomerId = 100,
            ProductId = 200,
            Quantity = 2,
            UnitPrice = 50m,
            PaymentMethod = "CreditCard",
            Status = OrderStatus.Confirmed
        };

        var paymentResult = new PaymentResult
        {
            Success = true,
            TransactionId = "TXN123456",
            ErrorMessage = string.Empty
        };

        mockInventoryService.CheckAvailabilityAsync(order.ProductId, order.Quantity).Returns(true);
        mockInventoryService.ReserveStockAsync(order.ProductId, order.Quantity).Returns(true);
        mockPaymentService.ProcessPaymentAsync(order.TotalAmount, order.PaymentMethod).Returns(paymentResult);
        mockOrderRepository.SaveAsync(Arg.Any<Order>()).Returns(confirmedOrder);
        mockNotificationService.SendOrderConfirmationAsync(order.CustomerId, order.Id.ToString()).Returns(true);

        // Act
        var result = await orderProcessor.ProcessOrderAsync(order);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(OrderStatus.Confirmed);
        result.Id.Should().Be(1);
        await mockInventoryService.Received(1).CheckAvailabilityAsync(order.ProductId, order.Quantity);
        await mockInventoryService.Received(1).ReserveStockAsync(order.ProductId, order.Quantity);
        await mockPaymentService.Received(1).ProcessPaymentAsync(order.TotalAmount, order.PaymentMethod);
        await mockOrderRepository.Received(1).SaveAsync(Arg.Is<Order>(o => o.Status == OrderStatus.Confirmed));
        await mockNotificationService.Received(1).SendOrderConfirmationAsync(order.CustomerId, order.Id.ToString());
    }

    [Fact(DisplayName = "處理訂單: 信用卡付款方式的訂單處理，應成功確認訂單")]
    public async Task ProcessOrderAsync_信用卡付款方式的訂單處理_應成功確認訂單()
    {
        // Arrange
        var order = new Order
        {
            Id = 2,
            CustomerId = 101,
            ProductId = 201,
            Quantity = 1,
            UnitPrice = 100m,
            PaymentMethod = "CreditCard",
            Status = OrderStatus.Pending
        };

        var confirmedOrder = new Order
        {
            Id = 2,
            CustomerId = 101,
            ProductId = 201,
            Quantity = 1,
            UnitPrice = 100m,
            PaymentMethod = "CreditCard",
            Status = OrderStatus.Confirmed
        };

        var paymentResult = new PaymentResult { Success = true, TransactionId = "CC123" };

        mockInventoryService.CheckAvailabilityAsync(order.ProductId, order.Quantity).Returns(true);
        mockInventoryService.ReserveStockAsync(order.ProductId, order.Quantity).Returns(true);
        mockPaymentService.ProcessPaymentAsync(order.TotalAmount, order.PaymentMethod).Returns(paymentResult);
        mockOrderRepository.SaveAsync(Arg.Any<Order>()).Returns(confirmedOrder);
        mockNotificationService.SendOrderConfirmationAsync(order.CustomerId, order.Id.ToString()).Returns(true);

        // Act
        var result = await orderProcessor.ProcessOrderAsync(order);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(OrderStatus.Confirmed);
        await mockPaymentService.Received(1).ProcessPaymentAsync(order.TotalAmount, "CreditCard");
    }

    [Fact(DisplayName = "處理訂單: 大量商品訂單處理，應正確處理庫存檢查和保留")]
    public async Task ProcessOrderAsync_大量商品訂單處理_應正確處理庫存檢查和保留()
    {
        // Arrange
        var order = new Order
        {
            Id = 3,
            CustomerId = 102,
            ProductId = 202,
            Quantity = 100,
            UnitPrice = 10m,
            PaymentMethod = "BankTransfer",
            Status = OrderStatus.Pending
        };

        var confirmedOrder = new Order
        {
            Id = 3,
            CustomerId = 102,
            ProductId = 202,
            Quantity = 100,
            UnitPrice = 10m,
            PaymentMethod = "BankTransfer",
            Status = OrderStatus.Confirmed
        };

        var paymentResult = new PaymentResult { Success = true, TransactionId = "BT456" };

        mockInventoryService.CheckAvailabilityAsync(order.ProductId, order.Quantity).Returns(true);
        mockInventoryService.ReserveStockAsync(order.ProductId, order.Quantity).Returns(true);
        mockPaymentService.ProcessPaymentAsync(order.TotalAmount, order.PaymentMethod).Returns(paymentResult);
        mockOrderRepository.SaveAsync(Arg.Any<Order>()).Returns(confirmedOrder);
        mockNotificationService.SendOrderConfirmationAsync(order.CustomerId, order.Id.ToString()).Returns(true);

        // Act
        var result = await orderProcessor.ProcessOrderAsync(order);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(OrderStatus.Confirmed);
        await mockInventoryService.Received(1).CheckAvailabilityAsync(202, 100);
        await mockInventoryService.Received(1).ReserveStockAsync(202, 100);
    }

    #endregion

    #region 邊界條件測試

    [Fact(DisplayName = "處理訂單: 最小金額訂單處理，應成功處理極小金額的訂單")]
    public async Task ProcessOrderAsync_最小金額訂單處理_應成功處理極小金額的訂單()
    {
        // Arrange
        var order = new Order
        {
            Id = 4,
            CustomerId = 103,
            ProductId = 203,
            Quantity = 1,
            UnitPrice = 0.01m,
            PaymentMethod = "Cash",
            Status = OrderStatus.Pending
        };

        var confirmedOrder = new Order
        {
            Id = 4,
            CustomerId = 103,
            ProductId = 203,
            Quantity = 1,
            UnitPrice = 0.01m,
            PaymentMethod = "Cash",
            Status = OrderStatus.Confirmed
        };

        var paymentResult = new PaymentResult { Success = true, TransactionId = "CASH001" };

        mockInventoryService.CheckAvailabilityAsync(order.ProductId, order.Quantity).Returns(true);
        mockInventoryService.ReserveStockAsync(order.ProductId, order.Quantity).Returns(true);
        mockPaymentService.ProcessPaymentAsync(order.TotalAmount, order.PaymentMethod).Returns(paymentResult);
        mockOrderRepository.SaveAsync(Arg.Any<Order>()).Returns(confirmedOrder);
        mockNotificationService.SendOrderConfirmationAsync(order.CustomerId, order.Id.ToString()).Returns(true);

        // Act
        var result = await orderProcessor.ProcessOrderAsync(order);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(OrderStatus.Confirmed);
        await mockPaymentService.Received(1).ProcessPaymentAsync(0.01m, order.PaymentMethod);
    }

    [Fact(DisplayName = "處理訂單: 大額訂單處理，應成功處理大額付款")]
    public async Task ProcessOrderAsync_大額訂單處理_應成功處理大額付款()
    {
        // Arrange
        var order = new Order
        {
            Id = 5,
            CustomerId = 104,
            ProductId = 204,
            Quantity = 10,
            UnitPrice = 99999.99m,
            PaymentMethod = "BankTransfer",
            Status = OrderStatus.Pending
        };

        var confirmedOrder = new Order
        {
            Id = 5,
            CustomerId = 104,
            ProductId = 204,
            Quantity = 10,
            UnitPrice = 99999.99m,
            PaymentMethod = "BankTransfer",
            Status = OrderStatus.Confirmed
        };

        var paymentResult = new PaymentResult { Success = true, TransactionId = "BT999" };

        mockInventoryService.CheckAvailabilityAsync(order.ProductId, order.Quantity).Returns(true);
        mockInventoryService.ReserveStockAsync(order.ProductId, order.Quantity).Returns(true);
        mockPaymentService.ProcessPaymentAsync(order.TotalAmount, order.PaymentMethod).Returns(paymentResult);
        mockOrderRepository.SaveAsync(Arg.Any<Order>()).Returns(confirmedOrder);
        mockNotificationService.SendOrderConfirmationAsync(order.CustomerId, order.Id.ToString()).Returns(true);

        // Act
        var result = await orderProcessor.ProcessOrderAsync(order);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(OrderStatus.Confirmed);
        await mockPaymentService.Received(1).ProcessPaymentAsync(999999.90m, order.PaymentMethod);
    }

    [Fact(DisplayName = "處理訂單: 庫存剛好足夠的情況，應成功保留庫存並處理訂單")]
    public async Task ProcessOrderAsync_庫存剛好足夠的情況_應成功保留庫存並處理訂單()
    {
        // Arrange
        var order = new Order
        {
            Id = 6,
            CustomerId = 105,
            ProductId = 205,
            Quantity = 1,
            UnitPrice = 50m,
            PaymentMethod = "CreditCard",
            Status = OrderStatus.Pending
        };

        var confirmedOrder = new Order
        {
            Id = 6,
            CustomerId = 105,
            ProductId = 205,
            Quantity = 1,
            UnitPrice = 50m,
            PaymentMethod = "CreditCard",
            Status = OrderStatus.Confirmed
        };

        var paymentResult = new PaymentResult { Success = true, TransactionId = "CC789" };

        mockInventoryService.CheckAvailabilityAsync(order.ProductId, order.Quantity).Returns(true);
        mockInventoryService.ReserveStockAsync(order.ProductId, order.Quantity).Returns(true);
        mockPaymentService.ProcessPaymentAsync(order.TotalAmount, order.PaymentMethod).Returns(paymentResult);
        mockOrderRepository.SaveAsync(Arg.Any<Order>()).Returns(confirmedOrder);
        mockNotificationService.SendOrderConfirmationAsync(order.CustomerId, order.Id.ToString()).Returns(true);

        // Act
        var result = await orderProcessor.ProcessOrderAsync(order);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(OrderStatus.Confirmed);
        await mockInventoryService.Received(1).CheckAvailabilityAsync(order.ProductId, order.Quantity);
        await mockInventoryService.Received(1).ReserveStockAsync(order.ProductId, order.Quantity);
    }

    #endregion

    #region 異常處理測試

    [Fact(DisplayName = "處理訂單: null 訂單輸入，應拋出 ArgumentNullException")]
    public async Task ProcessOrderAsync_null訂單輸入_應拋出ArgumentNullException()
    {
        // Arrange
        Order? order = null;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => orderProcessor.ProcessOrderAsync(order!));

        exception.ParamName.Should().Be("order");
        await mockInventoryService.DidNotReceive().CheckAvailabilityAsync(Arg.Any<int>(), Arg.Any<int>());
        await mockInventoryService.DidNotReceive().ReserveStockAsync(Arg.Any<int>(), Arg.Any<int>());
        await mockPaymentService.DidNotReceive().ProcessPaymentAsync(Arg.Any<decimal>(), Arg.Any<string>());
        await mockOrderRepository.DidNotReceive().SaveAsync(Arg.Any<Order>());
        await mockNotificationService.DidNotReceive().SendOrderConfirmationAsync(Arg.Any<int>(), Arg.Any<string>());
    }

    [Fact(DisplayName = "處理訂單: 庫存不足情況，應拋出 InvalidOperationException")]
    public async Task ProcessOrderAsync_庫存不足情況_應拋出InvalidOperationException()
    {
        // Arrange
        var order = new Order
        {
            Id = 7,
            CustomerId = 106,
            ProductId = 206,
            Quantity = 5,
            UnitPrice = 20m,
            PaymentMethod = "CreditCard",
            Status = OrderStatus.Pending
        };

        mockInventoryService.CheckAvailabilityAsync(order.ProductId, order.Quantity).Returns(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => orderProcessor.ProcessOrderAsync(order));

        exception.Message.Should().Be("庫存不足");
        await mockInventoryService.Received(1).CheckAvailabilityAsync(order.ProductId, order.Quantity);
        await mockInventoryService.DidNotReceive().ReserveStockAsync(Arg.Any<int>(), Arg.Any<int>());
        await mockPaymentService.DidNotReceive().ProcessPaymentAsync(Arg.Any<decimal>(), Arg.Any<string>());
        await mockOrderRepository.DidNotReceive().SaveAsync(Arg.Any<Order>());
        await mockNotificationService.DidNotReceive().SendOrderConfirmationAsync(Arg.Any<int>(), Arg.Any<string>());
    }

    [Fact(DisplayName = "處理訂單: 無法保留庫存情況，應拋出 InvalidOperationException")]
    public async Task ProcessOrderAsync_無法保留庫存情況_應拋出InvalidOperationException()
    {
        // Arrange
        var order = new Order
        {
            Id = 8,
            CustomerId = 107,
            ProductId = 207,
            Quantity = 3,
            UnitPrice = 30m,
            PaymentMethod = "BankTransfer",
            Status = OrderStatus.Pending
        };

        mockInventoryService.CheckAvailabilityAsync(order.ProductId, order.Quantity).Returns(true);
        mockInventoryService.ReserveStockAsync(order.ProductId, order.Quantity).Returns(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => orderProcessor.ProcessOrderAsync(order));

        exception.Message.Should().Be("無法保留庫存");
        await mockInventoryService.Received(1).CheckAvailabilityAsync(order.ProductId, order.Quantity);
        await mockInventoryService.Received(1).ReserveStockAsync(order.ProductId, order.Quantity);
        await mockPaymentService.DidNotReceive().ProcessPaymentAsync(Arg.Any<decimal>(), Arg.Any<string>());
        await mockOrderRepository.DidNotReceive().SaveAsync(Arg.Any<Order>());
        await mockNotificationService.DidNotReceive().SendOrderConfirmationAsync(Arg.Any<int>(), Arg.Any<string>());
    }

    [Fact(DisplayName = "處理訂單: 付款失敗情況，應拋出 InvalidOperationException")]
    public async Task ProcessOrderAsync_付款失敗情況_應拋出InvalidOperationException()
    {
        // Arrange
        var order = new Order
        {
            Id = 9,
            CustomerId = 108,
            ProductId = 208,
            Quantity = 2,
            UnitPrice = 75m,
            PaymentMethod = "CreditCard",
            Status = OrderStatus.Pending
        };

        var paymentResult = new PaymentResult
        {
            Success = false,
            TransactionId = string.Empty,
            ErrorMessage = "信用卡被拒絕"
        };

        mockInventoryService.CheckAvailabilityAsync(order.ProductId, order.Quantity).Returns(true);
        mockInventoryService.ReserveStockAsync(order.ProductId, order.Quantity).Returns(true);
        mockPaymentService.ProcessPaymentAsync(order.TotalAmount, order.PaymentMethod).Returns(paymentResult);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => orderProcessor.ProcessOrderAsync(order));

        exception.Message.Should().Be("付款失敗：信用卡被拒絕");
        await mockInventoryService.Received(1).CheckAvailabilityAsync(order.ProductId, order.Quantity);
        await mockInventoryService.Received(1).ReserveStockAsync(order.ProductId, order.Quantity);
        await mockPaymentService.Received(1).ProcessPaymentAsync(order.TotalAmount, order.PaymentMethod);
        await mockOrderRepository.DidNotReceive().SaveAsync(Arg.Any<Order>());
        await mockNotificationService.DidNotReceive().SendOrderConfirmationAsync(Arg.Any<int>(), Arg.Any<string>());
    }

    [Fact(DisplayName = "處理訂單: 訂單保存失敗，應拋出相應的資料庫異常")]
    public async Task ProcessOrderAsync_訂單保存失敗_應拋出相應的資料庫異常()
    {
        // Arrange
        var order = new Order
        {
            Id = 10,
            CustomerId = 109,
            ProductId = 209,
            Quantity = 1,
            UnitPrice = 100m,
            PaymentMethod = "Cash",
            Status = OrderStatus.Pending
        };

        var paymentResult = new PaymentResult { Success = true, TransactionId = "CASH100" };
        var databaseException = new InvalidOperationException("資料庫連線失敗");

        mockInventoryService.CheckAvailabilityAsync(order.ProductId, order.Quantity).Returns(true);
        mockInventoryService.ReserveStockAsync(order.ProductId, order.Quantity).Returns(true);
        mockPaymentService.ProcessPaymentAsync(order.TotalAmount, order.PaymentMethod).Returns(paymentResult);
        mockOrderRepository.When(x => x.SaveAsync(Arg.Any<Order>())).Do(x => throw databaseException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => orderProcessor.ProcessOrderAsync(order));

        exception.Message.Should().Be("資料庫連線失敗");
        await mockInventoryService.Received(1).CheckAvailabilityAsync(order.ProductId, order.Quantity);
        await mockInventoryService.Received(1).ReserveStockAsync(order.ProductId, order.Quantity);
        await mockPaymentService.Received(1).ProcessPaymentAsync(order.TotalAmount, order.PaymentMethod);
        await mockOrderRepository.Received(1).SaveAsync(Arg.Is<Order>(o => o.Status == OrderStatus.Confirmed));
        await mockNotificationService.DidNotReceive().SendOrderConfirmationAsync(Arg.Any<int>(), Arg.Any<string>());
    }

    [Fact(DisplayName = "處理訂單: 通知發送失敗，應正常回傳訂單不受影響")]
    public async Task ProcessOrderAsync_通知發送失敗_應正常回傳訂單不受影響()
    {
        // Arrange
        var order = new Order
        {
            Id = 11,
            CustomerId = 110,
            ProductId = 210,
            Quantity = 1,
            UnitPrice = 80m,
            PaymentMethod = "BankTransfer",
            Status = OrderStatus.Pending
        };

        var confirmedOrder = new Order
        {
            Id = 11,
            CustomerId = 110,
            ProductId = 210,
            Quantity = 1,
            UnitPrice = 80m,
            PaymentMethod = "BankTransfer",
            Status = OrderStatus.Confirmed
        };

        var paymentResult = new PaymentResult { Success = true, TransactionId = "BT888" };

        mockInventoryService.CheckAvailabilityAsync(order.ProductId, order.Quantity).Returns(true);
        mockInventoryService.ReserveStockAsync(order.ProductId, order.Quantity).Returns(true);
        mockPaymentService.ProcessPaymentAsync(order.TotalAmount, order.PaymentMethod).Returns(paymentResult);
        mockOrderRepository.SaveAsync(Arg.Any<Order>()).Returns(confirmedOrder);
        mockNotificationService.SendOrderConfirmationAsync(order.CustomerId, order.Id.ToString()).Returns(false);

        // Act
        var result = await orderProcessor.ProcessOrderAsync(order);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(OrderStatus.Confirmed);
        await mockInventoryService.Received(1).CheckAvailabilityAsync(order.ProductId, order.Quantity);
        await mockInventoryService.Received(1).ReserveStockAsync(order.ProductId, order.Quantity);
        await mockPaymentService.Received(1).ProcessPaymentAsync(order.TotalAmount, order.PaymentMethod);
        await mockOrderRepository.Received(1).SaveAsync(Arg.Is<Order>(o => o.Status == OrderStatus.Confirmed));
        await mockNotificationService.Received(1).SendOrderConfirmationAsync(order.CustomerId, order.Id.ToString());
    }

    #endregion

    #region 業務流程順序驗證測試

    [Fact(DisplayName = "處理訂單: 驗證方法呼叫順序，應按正確順序呼叫各依賴服務方法")]
    public async Task ProcessOrderAsync_驗證方法呼叫順序_應按正確順序呼叫各依賴服務方法()
    {
        // Arrange
        var order = new Order
        {
            Id = 12,
            CustomerId = 111,
            ProductId = 211,
            Quantity = 2,
            UnitPrice = 60m,
            PaymentMethod = "CreditCard",
            Status = OrderStatus.Pending
        };

        var confirmedOrder = new Order
        {
            Id = 12,
            CustomerId = 111,
            ProductId = 211,
            Quantity = 2,
            UnitPrice = 60m,
            PaymentMethod = "CreditCard",
            Status = OrderStatus.Confirmed
        };

        var paymentResult = new PaymentResult { Success = true, TransactionId = "CC999" };

        mockInventoryService.CheckAvailabilityAsync(order.ProductId, order.Quantity).Returns(true);
        mockInventoryService.ReserveStockAsync(order.ProductId, order.Quantity).Returns(true);
        mockPaymentService.ProcessPaymentAsync(order.TotalAmount, order.PaymentMethod).Returns(paymentResult);
        mockOrderRepository.SaveAsync(Arg.Any<Order>()).Returns(confirmedOrder);
        mockNotificationService.SendOrderConfirmationAsync(order.CustomerId, order.Id.ToString()).Returns(true);

        // Act
        var result = await orderProcessor.ProcessOrderAsync(order);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(OrderStatus.Confirmed);

        Received.InOrder(async () =>
        {
            await mockInventoryService.CheckAvailabilityAsync(order.ProductId, order.Quantity);
            await mockInventoryService.ReserveStockAsync(order.ProductId, order.Quantity);
            await mockPaymentService.ProcessPaymentAsync(order.TotalAmount, order.PaymentMethod);
            await mockOrderRepository.SaveAsync(Arg.Is<Order>(o => o.Status == OrderStatus.Confirmed));
            await mockNotificationService.SendOrderConfirmationAsync(order.CustomerId, order.Id.ToString());
        });
    }

    [Fact(DisplayName = "處理訂單: 付款失敗時的資源清理，應拋出付款異常")]
    public async Task ProcessOrderAsync_付款失敗時的資源清理_應拋出付款異常()
    {
        // Arrange
        var order = new Order
        {
            Id = 13,
            CustomerId = 112,
            ProductId = 212,
            Quantity = 3,
            UnitPrice = 40m,
            PaymentMethod = "BankTransfer",
            Status = OrderStatus.Pending
        };

        var paymentResult = new PaymentResult
        {
            Success = false,
            TransactionId = string.Empty,
            ErrorMessage = "銀行轉帳失敗"
        };

        mockInventoryService.CheckAvailabilityAsync(order.ProductId, order.Quantity).Returns(true);
        mockInventoryService.ReserveStockAsync(order.ProductId, order.Quantity).Returns(true);
        mockPaymentService.ProcessPaymentAsync(order.TotalAmount, order.PaymentMethod).Returns(paymentResult);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => orderProcessor.ProcessOrderAsync(order));

        exception.Message.Should().Be("付款失敗：銀行轉帳失敗");

        // 驗證庫存已被保留但付款失敗後的狀態
        await mockInventoryService.Received(1).CheckAvailabilityAsync(order.ProductId, order.Quantity);
        await mockInventoryService.Received(1).ReserveStockAsync(order.ProductId, order.Quantity);
        await mockPaymentService.Received(1).ProcessPaymentAsync(order.TotalAmount, order.PaymentMethod);
        await mockOrderRepository.DidNotReceive().SaveAsync(Arg.Any<Order>());
        await mockNotificationService.DidNotReceive().SendOrderConfirmationAsync(Arg.Any<int>(), Arg.Any<string>());
    }

    #endregion
}